using System.Diagnostics;

using Game.Application;
using Game.Functional.Logic;
using Game.Functional.Models;
using Game.Terminal;

static GameViewModel Creategame()
{
	Position head = new(Random.Shared.Next(2, 18), Random.Shared.Next(2, 10));
	var world = Coordinator.CreateWorld(20, 12, head);
	GameRules rules = new(
		MoveIntervalSeconds: 0.2, FoodSpawnIntervalSeconds: 1,
		FoodSpawnCount: 2, FoodLimit: 8);
	return new(world, rules, Random.Shared);
}

Console.CursorVisible = false;
try
{
	while (true)
	{
		var game = Creategame();
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
			if (TerminalInput.ReadDirection() is { } direction)
			{
				game.RequestTurn(direction);
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
