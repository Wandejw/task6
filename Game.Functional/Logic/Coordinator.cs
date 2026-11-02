using System.Collections.Immutable;

using Game.Functional.Models;

namespace Game.Functional.Logic;

static class Coordinator
{
	public static WorldState RequestTurn(WorldState world, Direction requestedDirection)
	{
		var player = world.Player;
		var snake = player.Snake;
		if (player.PendingDirection.HasValue
			|| requestedDirection == snake.CurrentDirection
			|| requestedDirection == snake.CurrentDirection.Opposite)
		{
			return world;
		}

		var newPlayer = new PlayerState(
			snake, requestedDirection, player.MoveElapsedSeconds);

		return new WorldState(world.Width, world.Height, newPlayer, world.Foods, world.FoodSpawnElapsedSeconds);
	}

	public static (WorldState World, bool IsOver) Update(
		WorldState world,
		double elapsedSeconds,
		GameRules rules,
		ImmutableArray<int> randomValues)
	{
		var player = world.Player;
		double accumulatedSeconds = player.MoveElapsedSeconds + elapsedSeconds;
		bool shouldMove = accumulatedSeconds >= rules.MoveIntervalSeconds;
		double remainingSeconds = shouldMove
			? accumulatedSeconds - rules.MoveIntervalSeconds
			: accumulatedSeconds;

		world = new WorldState(world.Width, world.Height,
			new(player.Snake, player.PendingDirection, remainingSeconds),
			world.Foods, world.FoodSpawnElapsedSeconds + elapsedSeconds);

		if (shouldMove)
		{
			var snake = new SnakeState(player.Snake.Body,
				player.PendingDirection ?? player.Snake.CurrentDirection);
			var nextPosition = snake.Head + snake.CurrentDirection.Position;
			snake = world.Foods.Contains(nextPosition)
				? Movement.Grow(snake)
				: Movement.Move(snake);
			if (Collision.IsCollision(snake, world.Width, world.Height))
			{
				return (world, true);
			}

			var remainingFoods = world.Foods.Where(food => food != nextPosition).ToImmutableArray();
			world = new WorldState(world.Width, world.Height,
				new(snake, null, remainingSeconds), remainingFoods, world.FoodSpawnElapsedSeconds);
		}

		if (world.FoodSpawnElapsedSeconds < rules.FoodSpawnIntervalSeconds)
		{
			return (world, false);
		}

		var foods = world.Foods;
		if (foods.Length < rules.FoodLimit)
		{
			var freePositions = Spawn.FreePositions(world);
			int spawnCount = Math.Min(rules.FoodSpawnCount, freePositions.Length);
			foods = RefreshFood(foods, freePositions, randomValues.Take(spawnCount));
		}

		return (new WorldState(world.Width, world.Height, world.Player, foods,
			world.FoodSpawnElapsedSeconds - rules.FoodSpawnIntervalSeconds), false);

		static ImmutableArray<Position> RefreshFood(
			ImmutableArray<Position> currentFoods,
			ImmutableArray<Position> availablePositions,
			IEnumerable<int> values)
		{
			foreach (int value in values)
			{
				var position = availablePositions[value % availablePositions.Length];
				currentFoods = currentFoods.Add(position);
				availablePositions = availablePositions.Remove(position);
			}

			return currentFoods;
		}
	}
}
