namespace Game.Functional.Models;

class GameRules(
	double moveIntervalSeconds,
	double foodSpawnIntervalSeconds,
	int foodSpawnCount,
	int foodLimit)
{
	public double MoveIntervalSeconds { get; } = moveIntervalSeconds;
	public double FoodSpawnIntervalSeconds { get; } = foodSpawnIntervalSeconds;
	public int FoodSpawnCount { get; } = foodSpawnCount;
	public int FoodLimit { get; } = foodLimit;
}
