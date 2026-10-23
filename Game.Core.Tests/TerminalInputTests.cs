using Game.Terminal;

namespace Game.Core.Tests;

public class TerminalInputTests
{
	[Theory]
	[InlineData(ConsoleKey.A, GameOverChoice.Retry)]
	[InlineData(ConsoleKey.Escape, GameOverChoice.Exit)]
	public void MenuKeyMapsToChoice(ConsoleKey key, GameOverChoice expected)
		=> Assert.Equal(expected, TerminalInput.MapGameOverChoice(key));

	[Fact]
	public void EveryOtherKeyHasNoMenuChoice()
	{
		foreach (var key in Enum.GetValues<ConsoleKey>())
		{
			if (key is ConsoleKey.A or ConsoleKey.Escape)
			{
				continue;
			}

			Assert.Null(TerminalInput.MapGameOverChoice(key));
		}
	}

	[Theory]
	[InlineData(ConsoleKey.UpArrow, Direction.Up)]
	[InlineData(ConsoleKey.DownArrow, Direction.Down)]
	[InlineData(ConsoleKey.LeftArrow, Direction.Left)]
	[InlineData(ConsoleKey.RightArrow, Direction.Right)]
	public void ArrowKeyMapsToDirection(ConsoleKey key, Direction expected)
		=> Assert.Equal(expected, TerminalInput.MapDirection(key));

	[Fact]
	public void EveryNonArrowKeyHasNoDirection()
	{
		foreach (var key in Enum.GetValues<ConsoleKey>())
		{
			if (key is ConsoleKey.UpArrow or ConsoleKey.DownArrow
				or ConsoleKey.LeftArrow or ConsoleKey.RightArrow)
			{
				continue;
			}

			Assert.Null(TerminalInput.MapDirection(key));
		}
	}
}
