using Game.Core;
using Game.Terminal;

using System.Diagnostics;
static Game.Core.Game Creategame()
{
	Board board = new(20, 12);
	Position head = new(Random.Shared.Next(2, 18), Random.Shared.Next(2, 10));
	Snake snake = new([head, new(head.X - 1, head.Y), new(head.X - 2, head.Y)], Direction.Right, 0.2);
	Player player = new("Player", snake);
	return new(board, [], [player], 1, 2, 8);
}

while (true)
{
	var game = Creategame();
	Console.Clear();
	TerminalRenderer.DrawBoard(game);
	TerminalRenderer.DrawInfo(game);

	var stopwatch = Stopwatch.StartNew();
	double previousSeconds = stopwatch.Elapsed.TotalSeconds;
	double accumulatorSeconds = 0;
	while (!game.IsOver)
	{
		double currentSeconds = stopwatch.Elapsed.TotalSeconds;
		double elapsedSeconds = currentSeconds - previousSeconds;
		if (TerminalInput.ReadDirection() is { } direction)
		{
			_ = game.RequestTurn(game.Players[0], direction);
		}

		previousSeconds = currentSeconds;
		accumulatorSeconds += elapsedSeconds;
		bool needsRedraw = accumulatorSeconds >= 0.02;
		while (accumulatorSeconds >= 0.02 && !game.IsOver)
		{
			game.Step(0.02);
			accumulatorSeconds -= 0.02;
		}

		if (needsRedraw)
		{
			Console.SetCursorPosition(0, 0);
			TerminalRenderer.DrawBoard(game);
			TerminalRenderer.DrawInfo(game);
		}
	}

	TerminalRenderer.DrawGameOver(game);
	var choice = TerminalInput.ReadGameOverChoice();
	while (choice == null)
	{
		choice = TerminalInput.ReadGameOverChoice();
	}

	if (choice == GameOverChoice.Exit)
	{
		break;
	}
}
