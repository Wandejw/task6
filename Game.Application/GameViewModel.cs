using System.Collections.Immutable;

using Game.Functional.Logic;
using Game.Functional.Models;

namespace Game.Application;

public class GameViewModel(WorldState initialWorld, GameRules gameRules, Random randomSource)
{
	readonly GameRules rules = gameRules;
	readonly Random random = randomSource;

	public WorldState World { get; private set; } = initialWorld;
	public bool IsOver { get; private set; }

	public void RequestTurn(Direction direction)
		=> World = Coordinator.RequestTurn(World, direction);
	public void Advance(double elapsedSeconds)
	{
		var values = ImmutableArray.CreateBuilder<int>(rules.FoodSpawnCount);
		for (int i = 0; i < rules.FoodSpawnCount; i++)
		{
			values.Add(random.Next());
		}

		var randomValues = values.ToImmutable();
		(World, IsOver) = Coordinator.Update(World, elapsedSeconds, rules, randomValues);
	}
}
