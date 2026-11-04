using System.Collections.Immutable;

namespace Game.Functional.Models;

public record SnakeState(ImmutableArray<Position> Body, Direction CurrentDirection)
{
	public Position Head => Body[0];
}
