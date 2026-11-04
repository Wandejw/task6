using Game.Functional.Models;

namespace Game.Terminal;

public static class TerminalInput
{
	public static Direction? ReadDirection()
		=> ReadKeyOrNull()?.MapDirection();

	public static GameOverChoice? ReadGameOverChoice()
		=> ReadKeyOrNull()?.MapGameOverChoice();

	public static ConsoleKey? ReadKeyOrNull()
	=> Console.KeyAvailable
	? Console.ReadKey(true).Key
	: null;

	extension(ConsoleKey key)
	{
		public Direction? MapDirection() => key switch
		{
			ConsoleKey.UpArrow => Direction.Up,
			ConsoleKey.DownArrow => Direction.Down,
			ConsoleKey.RightArrow => Direction.Right,
			ConsoleKey.LeftArrow => Direction.Left,
			_ => null
		};

		public GameOverChoice? MapGameOverChoice() => key switch
		{
			ConsoleKey.A => GameOverChoice.Retry,
			ConsoleKey.Escape => GameOverChoice.Exit,
			_ => null
		};

		public Skill? MapSkill() => key switch
		{
			ConsoleKey.R => Skill.Sprint,
			_ => null
		};
	}
}
public enum GameOverChoice
{
	Retry,
	Exit
}

