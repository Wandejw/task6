namespace Game.Core;

public class Game(
	Board board,
	List<Food> foods,
	List<Player> players,
	double foodSpawnIntervalSeconds,
	int foodSpawnPerPlayer,
	int foodLimitPerPlayer
)
{
	readonly int foodSpawnPerPlayer = foodSpawnPerPlayer;
	readonly int foodLimitPerPlayer = foodLimitPerPlayer;
	readonly double foodSpawnIntervalSeconds = foodSpawnIntervalSeconds;
	public Board Board { get; } = board;
	readonly List<Food> foodList = [.. foods];
	readonly List<Player> playerList = [.. players];

	public IReadOnlyList<Food> Foods => foodList.AsReadOnly();
	public IReadOnlyList<Player> Players => playerList.AsReadOnly();

	public bool IsOver { get; private set; } = false;
	double foodSpawnElapsedSeconds = 0;

	public bool RequestTurn(Player player, Direction direction)
	=> !IsOver
	&& playerList.Contains(player)
	&& player.IsAlive
	&& player.Snake.RequestTurn(direction);

	bool IspositionAvailable(Position position)
	{
		if (!Board.Contains(position))
		{
			return false;
		}

		foreach (var player in playerList)
		{
			if (player.Snake.Body.Contains(position))
			{
				return false;
			}
		}

		foreach (var food in foodList)
		{
			if (food.Position.Equals(position))
			{
				return false;
			}
		}

		return true;
	}

	Position? RandomPosition()
	{
		List<Position> availablePositions = [];
		for (int x = 0; x < Board.Width; x++)
		{
			for (int y = 0; y < Board.Height; y++)
			{
				var pos = new Position(x, y);
				if (IspositionAvailable(pos))
				{
					availablePositions.Add(pos);
				}
			}
		}

		if (availablePositions.Count > 0)
		{
			Random random = new();
			return availablePositions[random.Next(availablePositions.Count)];
		}

		return null;
	}

	void IfSpawnFood()
	{
		if (foodSpawnElapsedSeconds >= foodSpawnIntervalSeconds)
		{
			foodSpawnElapsedSeconds -= foodSpawnIntervalSeconds;
			for (int i = 0; i < foodSpawnPerPlayer * playerList.Count; i++)
			{
				if (foodList.Count < foodLimitPerPlayer * playerList.Count)
				{
					var newFoodPosition = RandomPosition();
					if (newFoodPosition.HasValue)
					{
						foodList.Add(new Food(newFoodPosition.Value));
					}
				}
			}
		}
	}

	void UpdatePlayerStates(Player player, double elapsedSeconds)
	{
		var snake = player.Snake;
		if (!player.IsAlive)
		{
			return;
		}

		if (!snake.AdvanceTime(elapsedSeconds))
		{
			return;
		}

		var nextPosition = snake.GetNextPosition();
		int foodIndex = -1;
		for (int i = 0; i < foodList.Count; i++)
		{
			if (foodList[i].Position.Equals(nextPosition))
			{
				foodIndex = i;
				break;
			}
		}

		bool willGrow = foodIndex != -1;
		if (HasCollision(player, nextPosition, willGrow))
		{
			player.Die();
			return;
		}

		if (willGrow)
		{
			foodList.RemoveAt(foodIndex);
			snake.Grow(nextPosition);
			player.IncreaseScore(3);
		}
		else
		{
			snake.Move(nextPosition);
		}

		bool HasCollision(Player movingPlayer, Position nextPosition, bool willGrow)
		{
			if (!Board.Contains(nextPosition))
			{
				return true;
			}

			List<Position> occupiedPositions = [];
			foreach (var player in playerList)
			{
				for (int i = 0; i < player.Snake.Body.Count; i++)
				{
					var pos = player.Snake.Body[i];
					if (!willGrow && player == movingPlayer && i == player.Snake.Body.Count - 1)
					{
						continue;
					}

					occupiedPositions.Add(pos);
				}
			}

			return occupiedPositions.Contains(nextPosition);
		}
	}

	public void Step(double elapsedSeconds)
	{
		foodSpawnElapsedSeconds += elapsedSeconds;
		IsOver = true;
		foreach (var player in playerList)
		{
			UpdatePlayerStates(player, elapsedSeconds);
			if (player.IsAlive)
			{
				IsOver = false;
			}
		}

		IfSpawnFood();
	}
}
