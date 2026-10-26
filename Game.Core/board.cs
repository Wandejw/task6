namespace Game.Core;

public record class Board(int Width, int Height)
{
	public bool Contains(Position position)
	=> position.X >= 0
	&& position.X < Width
	&& position.Y >= 0
	&& position.Y < Height;

}

