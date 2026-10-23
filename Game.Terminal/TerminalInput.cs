using System.Reflection.Metadata.Ecma335;

using Game.Core;
namespace Game.Terminal;

public class TerminalInput
{
	public static Direction? ReadDirection()
		=> Console.KeyAvailable
		? MapDirection(Console.ReadKey(true).Key)
		: null;

	public static GameOverChoice? ReadGameOverChoice()
		=> Console.KeyAvailable
		? MapGameOverChoice(Console.ReadKey(true).Key)
		: null;

	public static Direction? MapDirection(ConsoleKey key)
	=> key switch
	{
		ConsoleKey.UpArrow => Direction.Up,
		ConsoleKey.DownArrow => Direction.Down,
		ConsoleKey.RightArrow => Direction.Right,
		ConsoleKey.LeftArrow => Direction.Left,
		_ => null
	};

	public static GameOverChoice? MapGameOverChoice(ConsoleKey key) => key switch
	{
		ConsoleKey.A => GameOverChoice.Retry,
		ConsoleKey.Escape => GameOverChoice.Exit,
		_ => null
	};
}

public enum GameOverChoice
{
	Retry,
	Exit
}

