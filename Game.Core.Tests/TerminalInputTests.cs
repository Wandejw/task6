using Game.Terminal;

using FunctionalDirection = global::Game.Functional.Models.Direction;

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
	[InlineData(ConsoleKey.UpArrow, FunctionalDirection.Up)]
	[InlineData(ConsoleKey.DownArrow, FunctionalDirection.Down)]
	[InlineData(ConsoleKey.LeftArrow, FunctionalDirection.Left)]
	[InlineData(ConsoleKey.RightArrow, FunctionalDirection.Right)]
	public void ArrowKeyMapsToDirection(ConsoleKey key, FunctionalDirection expected)
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
