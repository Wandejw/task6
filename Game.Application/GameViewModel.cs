using System.Collections.Immutable;

using Game.Functional.Logic;
using Game.Functional.Models;

namespace Game.Application;

public record GameViewModel(WorldState World, GameRules Rules, Random RandomSource)
{
	public WorldState World { get; private set; } = World;
	public bool IsOver { get; private set; }

	public void RequestTurn(Direction direction)
		=> World = Coordinator.RequestTurn(World, direction);
	public void RequestSprint()
		=> World = Coordinator.RequestSprint(World, Rules);
	public void Advance(double elapsedSeconds)
	{
		var values = ImmutableArray.CreateBuilder<int>(Rules.FoodSpawnCount);
		for (int i = 0; i < Rules.FoodSpawnCount; i++)
		{
			values.Add(RandomSource.Next());
		}

		var randomValues = values.ToImmutable();
		(World, IsOver) = Coordinator.Update(World, elapsedSeconds, Rules, randomValues);
	}
}
