using System.Diagnostics;

namespace Game.Functional.Models;

public readonly record struct Position(int X, int Y)
{
	public static Position operator +(Position a, Position b)
		=> new(a.X + b.X, a.Y + b.Y);
}

public enum Direction
{
	Up,
	Down,
	Left,
	Right
}

public static class PositionExtensions
{
	extension(Direction dir)
	{
		public Position Position => dir switch
		{
			Direction.Up => new(0, -1),
			Direction.Down => new(0, 1),
			Direction.Left => new(-1, 0),
			Direction.Right => new(1, 0),
			_ => throw new UnreachableException(),
		};

		public Direction Opposite => dir switch
		{
			Direction.Up => Direction.Down,
			Direction.Down => Direction.Up,
			Direction.Left => Direction.Right,
			Direction.Right => Direction.Left,
			_ => throw new UnreachableException(),
		};
	}
}
