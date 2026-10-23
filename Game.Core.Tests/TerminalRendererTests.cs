using Game.Terminal;

namespace Game.Core.Tests;

[CollectionDefinition("Console output", DisableParallelization = true)]
public class ConsoleOutputTestGroup
{
}

[Collection("Console output")]
public class TerminalRendererTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(9)]
	public void GameOverShowsFinalScoreAndRetryKeysWithoutResettingGame(int score)
	{
		var snake = new Snake([new(3, 1)], Direction.Right, 0.25);
		var player = new Player("Player", snake);
		player.IncreaseScore(score);
		var game = new Game(new Board(4, 3), [], [player], 1, 2, 8);
		game.Step(0.25);
		Assert.True(game.IsOver);
		Assert.False(player.IsAlive);
		using var output = new StringWriter();
		var originalOutput = Console.Out;

		try
		{
			Console.SetOut(output);
			TerminalRenderer.DrawGameOver(game);
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		string expected = string.Join(Environment.NewLine,
			"Game Over!", $"Final Score: {score}", "按A重新开始,按ESC退出", "");
		Assert.Equal(expected, output.ToString());
		Assert.Equal(score, player.Score);
		Assert.True(game.IsOver);
		Assert.False(player.IsAlive);
		Assert.Equal<Position>([new(3, 1)], snake.Body);
		Assert.Same(player, Assert.Single(game.Players));
		Assert.Empty(game.Foods);
	}

	[Fact]
	public void InfoWithoutPlayersStillShowsInstructions()
	{
		var game = new Game(new Board(4, 3), [], [], 1, 2, 8);
		using var output = new StringWriter();
		var originalOutput = Console.Out;

		try
		{
			Console.SetOut(output);
			TerminalRenderer.DrawInfo(game);
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		Assert.Equal($"操作：方向键移动{Environment.NewLine}", output.ToString());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(6)]
	public void InfoShowsCurrentScoreAndDirectionInstructions(int score)
	{
		var player = new Player("Player", new Snake([new(1, 1)], Direction.Right, 0.25));
		player.IncreaseScore(score);
		var game = new Game(new Board(4, 3), [], [player], 1, 2, 8);
		using var output = new StringWriter();
		var originalOutput = Console.Out;

		try
		{
			Console.SetOut(output);
			TerminalRenderer.DrawInfo(game);
			Assert.Contains($"Score: {score}{Environment.NewLine}", output.ToString());
			Assert.Contains($"操作：方向键移动{Environment.NewLine}", output.ToString());

			player.IncreaseScore(3);
			output.GetStringBuilder().Length = 0;
			TerminalRenderer.DrawInfo(game);
			Assert.Contains($"Score: {score + 3}{Environment.NewLine}", output.ToString());
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		Assert.Equal(score + 3, player.Score);
		Assert.True(player.IsAlive);
		Assert.False(game.IsOver);
	}

	[Fact]
	public void BoardShowsHeadBodyAndEveryFoodWithoutChangingState()
	{
		Position[] initialBody = [new(2, 1), new(1, 1), new(0, 1)];
		var snake = new Snake([.. initialBody], Direction.Right, 0.25);
		var player = new Player("Player", snake);
		Food[] foods = [new(new(3, 0)), new(new(0, 2))];
		var game = new Game(new Board(4, 3), [.. foods], [player], 1, 2, 8);
		using var output = new StringWriter();
		var originalOutput = Console.Out;

		try
		{
			Console.SetOut(output);
			TerminalRenderer.DrawBoard(game);
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		string expected = string.Join(Environment.NewLine,
			"############",
			"##      * ##",
			"##o o @   ##",
			"##*       ##",
			"############", "");
		Assert.Equal(expected, output.ToString());
		Assert.Equal(initialBody, snake.Body);
		Assert.Equal(foods, game.Foods);
		Assert.Same(player, Assert.Single(game.Players));
		Assert.Equal(0, player.Score);
		Assert.True(player.IsAlive);
		Assert.Equal(Direction.Right, snake.CurrentDirection);
		Assert.False(game.IsOver);
	}

	[Theory]
	[InlineData(4, 3, "############\n##        ##\n##        ##\n##        ##\n############\n")]
	[InlineData(1, 1, "######\n##  ##\n######\n")]
	[InlineData(2, 4, "########\n##    ##\n##    ##\n##    ##\n##    ##\n########\n")]
	public void EmptyBoardMatchesExpectedFrame(int width, int height, string expected)
	{
		var board = new Board(width, height);
		var game = new Game(board, [], [], 1, 2, 8);
		using var output = new StringWriter();
		var originalOutput = Console.Out;

		try
		{
			Console.SetOut(output);
			TerminalRenderer.DrawBoard(game);
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		Assert.Equal(expected.Replace("\n", Environment.NewLine), output.ToString());
		Assert.Same(board, game.Board);
		Assert.Empty(game.Players);
		Assert.Empty(game.Foods);
		Assert.False(game.IsOver);
	}
}
