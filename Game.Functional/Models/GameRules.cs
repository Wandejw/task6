namespace Game.Functional.Models;

public record GameRules(
	double MoveIntervalSeconds,
	double FoodSpawnIntervalSeconds,
	int FoodSpawnCount,
	int FoodLimit,
	double SprintDurationSeconds,
	double SprintCooldownSeconds,
	double SprintSpeedMultiplier);
