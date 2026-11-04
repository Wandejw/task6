using System.Collections.Immutable;

namespace Game.Functional.Models;

public record WorldState(
	int Width,
	int Height,
	PlayerState Player,
	ImmutableArray<Position> Foods,
	double FoodSpawnElapsedSeconds = 0);
