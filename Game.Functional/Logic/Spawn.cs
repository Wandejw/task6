using System.Collections.Immutable;

using Game.Functional.Models;

namespace Game.Functional.Logic;

static class Spawn
{
	public static ImmutableArray<Position> FreePositions(WorldState world)
	{
		var positions = ImmutableArray.CreateBuilder<Position>();
		for (int x = 0; x < world.Width; x++)
		{
			for (int y = 0; y < world.Height; y++)
			{
				positions.Add(new Position(x, y));
			}
		}

		return [.. positions
		.Where(position
		=> !world.Player.Snake.Body.Contains(position)
		&& !world.Foods.Contains(position))];
	}
}
