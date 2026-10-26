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
	=> !foodList.Contains(new Food(position))
	&& !playerList.Any(player => player.Snake.Body.Contains(position))
	&& Board.Contains(position);

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

		return availablePositions.Count > 0 ?
		availablePositions[new Random().Next(availablePositions.Count)] : null;
	}

	void IfSpawnFood()
	{
		if (foodSpawnElapsedSeconds < foodSpawnIntervalSeconds)
		{
			return;
		}

		foodSpawnElapsedSeconds -= foodSpawnIntervalSeconds;
		for (int i = 0;
		i < foodSpawnPerPlayer * playerList.Count
		&& foodList.Count < foodLimitPerPlayer * playerList.Count;
		i++)
		{
			if (RandomPosition() is { } foodPosition)
			{
				foodList.Add(new(foodPosition));
			}
		}
	}

	void UpdatePlayerStates(Player player, double elapsedSeconds)
	{
		var snake = player.Snake;
		if (!player.IsAlive || !snake.AdvanceTime(elapsedSeconds))
		{
			return;
		}

		var nextPosition = snake.GetNextPosition();
		int foodIndex = foodList.FindIndex(food => food.Position == nextPosition);
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
		=> !Board.Contains(nextPosition) || playerList.Any(player =>
		{
			var body = player.Snake.Body;
			int checkCount = !willGrow && player == movingPlayer
			? body.Count - 1
			: body.Count;
			return body.Take(checkCount).Contains(nextPosition);
		});
	}

	public void Step(double elapsedSeconds)
	{
		foodSpawnElapsedSeconds += elapsedSeconds;
		foreach (var player in playerList)
		{
			UpdatePlayerStates(player, elapsedSeconds);
		}

		IsOver = !playerList.Any(player => player.IsAlive);
		IfSpawnFood();
	}
}
