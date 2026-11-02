using System.Collections.Immutable;

namespace Game.Functional.Models;

class WorldState(
	int iniWidth,
	int iniHeight,
	PlayerState iniPlayer,
	ImmutableArray<Position> iniFoods,
	double foodSpawnElapsedSeconds = 0)
{
	public int Width { get; } = iniWidth;
	public int Height { get; } = iniHeight;
	public PlayerState Player { get; } = iniPlayer;
	public ImmutableArray<Position> Foods { get; } = iniFoods;
	public double FoodSpawnElapsedSeconds { get; } = foodSpawnElapsedSeconds;
}
