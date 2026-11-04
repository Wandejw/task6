using System.Collections.Immutable;

using Game.Functional.Models;

namespace Game.Functional.Logic;

public static class Coordinator
{
	public static WorldState CreateWorld(int width, int height, Position head)
	{
		SnakeState snake = new(
			[head, new(head.X - 1, head.Y), new(head.X - 2, head.Y)], Direction.Right);
		PlayerState player = new(snake, null, 0, new(0, 0));
		return new(width, height, player, []);
	}

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

		var newPlayer = player with
		{
			PendingDirection = requestedDirection
		};

		return world with
		{
			Player = newPlayer
		};
	}

	public static WorldState RequestSprint(WorldState world, GameRules rules)
	{
		var player = world.Player;

		var newSprint = player.Sprint with
		{
			SprintRemainingSeconds = rules.SprintDurationSeconds,
			CooldownRemainingSeconds = 0
		};

		var newPlayer = player with
		{
			Sprint = newSprint
		};
		return player.Sprint.CooldownRemainingSeconds > 0 || player.Sprint.SprintRemainingSeconds > 0
		? world
		: world with { Player = newPlayer };
	}

	public static (WorldState World, bool IsOver) Update(
		WorldState world,
		double elapsedSeconds,
		GameRules rules,
		ImmutableArray<int> randomValues)
	{
		var player = world.Player;
		var sprint = player.Sprint;
		double moveIntervalSeconds = sprint.SprintRemainingSeconds > 0
			? rules.MoveIntervalSeconds / rules.SprintSpeedMultiplier
			: rules.MoveIntervalSeconds;
		double accumulatedSeconds = player.MoveElapsedSeconds + elapsedSeconds;
		bool shouldMove = accumulatedSeconds >= moveIntervalSeconds;
		double remainingSeconds = shouldMove
			? accumulatedSeconds - moveIntervalSeconds
			: accumulatedSeconds;
		var newSprint = sprint;

		if (sprint.SprintRemainingSeconds > 0)
		{
			double sprintRemainingSeconds = sprint.SprintRemainingSeconds - elapsedSeconds;
			newSprint = sprint with
			{
				SprintRemainingSeconds = Math.Max(0, sprintRemainingSeconds),
				CooldownRemainingSeconds = sprintRemainingSeconds <= 0
					? Math.Max(0, rules.SprintCooldownSeconds + sprintRemainingSeconds)
					: 0
			};
		}
		else if (sprint.CooldownRemainingSeconds > 0)
		{
			newSprint = sprint with
			{
				CooldownRemainingSeconds = Math.Max(0, sprint.CooldownRemainingSeconds - elapsedSeconds)
			};
		}

		world = world with
		{
			Player = player with
			{
				MoveElapsedSeconds = remainingSeconds,
				Sprint = newSprint
			},
			FoodSpawnElapsedSeconds = world.FoodSpawnElapsedSeconds + elapsedSeconds
		};

		if (shouldMove)
		{
			var snake = player.Snake with
			{
				CurrentDirection = player.PendingDirection ?? player.Snake.CurrentDirection
			};

			var nextPosition = snake.Head + snake.CurrentDirection.Position;
			snake = world.Foods.Contains(nextPosition)
				? Movement.Grow(snake)
				: Movement.Move(snake);
			if (Collision.IsCollision(snake, world.Width, world.Height))
			{
				return (world, true);
			}

			var remainingFoods = world.Foods.Where(food => food != nextPosition).ToImmutableArray();
			world = world with
			{
				Player = world.Player with
				{
					Snake = snake,
					PendingDirection = null
				},
				Foods = remainingFoods
			};
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

		return (world with
		{
			Foods = foods,
			FoodSpawnElapsedSeconds = world.FoodSpawnElapsedSeconds - rules.FoodSpawnIntervalSeconds
		}, false);

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
