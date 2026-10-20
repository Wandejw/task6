namespace Game.Core;

public class Board(int width, int height)
{
	public int Width { get; } = width;
	public int Height { get; } = height;

	public bool Contains(Position position)
	=> position.X >= 0
	&& position.X < Width
	&& position.Y >= 0
	&& position.Y < Height;

}

