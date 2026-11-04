using System.Diagnostics;
using System.Text.Json;

using Game.Application;
using Game.Functional.Logic;
using Game.Functional.Models;
using Game.Terminal;

static GameViewModel Creategame(GameConfig config)
{
	Position head = new(
	Random.Shared.Next(2, config.Width - 2),
	Random.Shared.Next(2, config.Height - 2));
	var world = Coordinator.CreateWorld(config.Width, config.Height, head);
	return new(world, config.Rules, Random.Shared);
}

string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
string json = File.ReadAllText(configPath);
var config = JsonSerializer.Deserialize<GameConfig>(json);
//将 json 中的文字，按 GameConfig 的结构转换成对象，用 config 保存结果。
if (config is null)
{
	Console.WriteLine("配置内容不能为 null。");
	return;
}
//顶层入口
Console.CursorVisible = false;
try
{
	while (true)
	{
		var game = Creategame(config);
		Console.Clear();
		TerminalRenderer.DrawBoard(game.World);
		TerminalRenderer.DrawInfo(game.World);

		var stopwatch = Stopwatch.StartNew();
		double previousSeconds = stopwatch.Elapsed.TotalSeconds;
		double accumulatorSeconds = 0;
		while (!game.IsOver)
		{
			double currentSeconds = stopwatch.Elapsed.TotalSeconds;
			double elapsedSeconds = currentSeconds - previousSeconds;
			if (TerminalInput.ReadKeyOrNull() is { } key)
			{
				if (key.MapDirection() is { } direction)
				{
					game.RequestTurn(direction);
				}

				if (key.MapSkill() is { })
				{
					game.RequestSprint();
				}
			}

			previousSeconds = currentSeconds;
			accumulatorSeconds += elapsedSeconds;
			bool needsRedraw = accumulatorSeconds >= 0.02;
			while (accumulatorSeconds >= 0.02 && !game.IsOver)
			{
				game.Advance(0.02);
				accumulatorSeconds -= 0.02;
			}

			if (needsRedraw)
			{
				Console.SetCursorPosition(0, 0);
				TerminalRenderer.DrawBoard(game.World);
				TerminalRenderer.DrawInfo(game.World);
			}

			Thread.Sleep(1);
		}

		TerminalRenderer.DrawGameOver(game.World);
		var choice = TerminalInput.ReadGameOverChoice();
		while (choice == null)
		{
			Thread.Sleep(10);
			choice = TerminalInput.ReadGameOverChoice();
		}

		if (choice == GameOverChoice.Exit)
		{
			break;
		}
	}
}
finally
{
	Console.CursorVisible = true;
}
