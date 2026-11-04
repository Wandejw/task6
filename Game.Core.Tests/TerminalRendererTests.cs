using System.Collections.Immutable;

using Game.Terminal;

using Models = Game.Functional.Models;

namespace Game.Core.Tests;

[CollectionDefinition("Console output", DisableParallelization = true)]
public class ConsoleOutputTestGroup
{
}

[Collection("Console output")]
public class TerminalRendererTests
{
	[Theory]
	[InlineData(1)]
	[InlineData(3)]
	public void GameOverShowsFinalLengthAndRetryKeysWithoutChangingWorld(int length)
	{
		var body = Enumerable.Range(0, length).Select(x => new Models.Position(x, 1)).ToImmutableArray();
		var world = CreateWorld(4, 3, body, []);
		string output = Capture(() => TerminalRenderer.DrawGameOver(world));

		string expected = string.Join(Environment.NewLine,
			"Game Over!", $"Final Length: {length}", "按A重新开始,按ESC退出", "");
		Assert.Equal(expected, output);
		Assert.Equal(body, world.Player.Snake.Body);
		Assert.Empty(world.Foods);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(3)]
	public void InfoShowsCurrentLengthAndDirectionInstructions(int length)
	{
		var body = Enumerable.Range(0, length).Select(x => new Models.Position(x, 1)).ToImmutableArray();
		var world = CreateWorld(4, 3, body, []);
		string output = Capture(() => TerminalRenderer.DrawInfo(world));

		Assert.Equal($"操作：方向键移动{Environment.NewLine}Length: {length}{Environment.NewLine}", output);
		Assert.Equal(length, world.Player.Snake.Body.Length);
	}

	[Fact]
	public void BoardShowsHeadBodyAndEveryFoodWithoutChangingWorld()
	{
		ImmutableArray<Models.Position> body = [new(2, 1), new(1, 1), new(0, 1)];
		ImmutableArray<Models.Position> foods = [new(3, 0), new(0, 2)];
		var world = CreateWorld(4, 3, body, foods);
		string output = Capture(() => TerminalRenderer.DrawBoard(world));

		string expected = string.Join(Environment.NewLine,
			"############",
			"##      * ##",
			"##o o @   ##",
			"##*       ##",
			"############", "");
		Assert.Equal(expected, output);
		Assert.Equal(body, world.Player.Snake.Body);
		Assert.Equal(foods, world.Foods);
		Assert.Equal(Models.Direction.Right, world.Player.Snake.CurrentDirection);
	}

	[Theory]
	[InlineData(4, 3, "############\n##@       ##\n##        ##\n##        ##\n############\n")]
	[InlineData(1, 1, "######\n##@ ##\n######\n")]
	[InlineData(2, 4, "########\n##@   ##\n##    ##\n##    ##\n##    ##\n########\n")]
	public void BoardMatchesConfiguredDimensions(int width, int height, string expected)
	{
		var world = CreateWorld(width, height, [new(0, 0)], []);
		string output = Capture(() => TerminalRenderer.DrawBoard(world));

		Assert.Equal(expected.Replace("\n", Environment.NewLine), output);
		Assert.Equal(width, world.Width);
		Assert.Equal(height, world.Height);
	}

	static Models.WorldState CreateWorld(int width, int height,
		ImmutableArray<Models.Position> body, ImmutableArray<Models.Position> foods)
		=> new(width, height, new(new(body, Models.Direction.Right), null, 0), foods);

	static string Capture(Action draw)
	{
		using var output = new StringWriter();
		var originalOutput = Console.Out;
		try
		{
			Console.SetOut(output);
			draw();
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		return output.ToString();
	}
}
