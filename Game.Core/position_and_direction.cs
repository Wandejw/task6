using System.Diagnostics;

namespace Game.Core;

public readonly struct Position(int x, int y)
{
	public int X { get; } = x;
	public int Y { get; } = y;

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
	}
}
