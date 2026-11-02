using System.Collections.Immutable;

namespace Game.Functional.Models;

class SnakeState(ImmutableArray<Position> initialBody, Direction initialDirection)
{
	public Position Head { get; } = initialBody[0];
	public Direction CurrentDirection { get; } = initialDirection;
	public ImmutableArray<Position> Body { get; } = initialBody;
}
