namespace Game.Core.Tests;

public class CoreRuleTests
{
	[Theory]
	[InlineData(0, 0, true)]
	[InlineData(3, 2, true)]
	[InlineData(-1, 1, false)]
	[InlineData(1, -1, false)]
	[InlineData(4, 1, false)]
	[InlineData(1, 3, false)]
	public void BoardChecksAllFourEdges(int x, int y, bool expected)
	{
		var board = new Board(4, 3);

		Assert.Equal(expected, board.Contains(new Position(x, y)));
	}

	[Theory]
	[InlineData(Direction.Up, 2, 1)]
	[InlineData(Direction.Down, 2, 3)]
	[InlineData(Direction.Left, 1, 2)]
	[InlineData(Direction.Right, 3, 2)]
	public void NextPositionMatchesDirection(Direction direction, int x, int y)
	{
		var snake = new Snake([new(2, 2)], direction, 0.25);

		Assert.Equal(new Position(x, y), snake.GetNextPosition());
		Assert.Equal<Position>([new(2, 2)], snake.Body);
	}

	[Fact]
	public void FixedStepsAccumulateBeforeMovementAndPreserveRemainder()
	{
		var player = new Player("Player1", new Snake([new(1, 1)], Direction.Right, 0.25));
		var game = new Game(new Board(8, 3), [], [player], 10, 2, 8);

		game.Step(0.125);
		Assert.Equal(new Position(1, 1), player.Snake.Body[0]);
		game.Step(0.125);
		Assert.Equal(new Position(2, 1), player.Snake.Body[0]);
		game.Step(0.125);
		Assert.Equal(new Position(2, 1), player.Snake.Body[0]);
		game.Step(0.125);
		Assert.Equal(new Position(3, 1), player.Snake.Body[0]);
		Assert.True(player.IsAlive);
	}

	[Fact]
	public void GameTurnRequestIsBufferedUntilMovement()
	{
		var player = new Player("Player1", new Snake([new(1, 1), new(0, 1)],
			Direction.Right, 0.25));
		var game = new Game(new Board(4, 3), [], [player], 10, 2, 8);

		Assert.False(game.RequestTurn(player, Direction.Left));
		Assert.False(game.RequestTurn(player, Direction.Right));
		Assert.True(game.RequestTurn(player, Direction.Up));
		Assert.False(game.RequestTurn(player, Direction.Down));
		Assert.Equal(new Position(1, 1), player.Snake.Body[0]);
		Assert.Equal(Direction.Right, player.Snake.CurrentDirection);

		game.Step(0.25);

		Assert.Equal<Position>([new(1, 0), new(1, 1)], player.Snake.Body);
		Assert.Equal(Direction.Up, player.Snake.CurrentDirection);
		Assert.True(game.RequestTurn(player, Direction.Left));
	}

	[Fact]
	public void CollisionWithOwnBodyEndsGameWithoutMoving()
	{
		List<Position> body =
		[
			new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(3, 1), new(3, 2)
		];
		var player = new Player("Player1", new Snake(body, Direction.Right, 0.25));
		var game = new Game(new Board(4, 4), [], [player], 10, 2, 8);

		game.Step(0.25);

		Assert.False(player.IsAlive);
		Assert.True(game.IsOver);
		Assert.Equal(body, player.Snake.Body);
		Assert.Equal(0, player.Score);
		Assert.False(game.RequestTurn(player, Direction.Up));
	}

	[Fact]
	public void MovingIntoVacatedTailCellIsAllowed()
	{
		var player = new Player("Player1", new Snake(
			[new(1, 1), new(1, 2), new(2, 2), new(2, 1)], Direction.Right, 0.25));
		var game = new Game(new Board(4, 4), [], [player], 10, 2, 8);

		game.Step(0.25);

		Assert.True(player.IsAlive);
		Assert.False(game.IsOver);
		Assert.Equal<Position>([new(2, 1), new(1, 1), new(1, 2), new(2, 2)],
			player.Snake.Body);
		Assert.Equal(0, player.Score);
	}

	[Fact]
	public void FoodSpawnsOnScheduleOnlyInDistinctFreeCells()
	{
		var player = new Player("Player1", new Snake([new(1, 1)], Direction.Right, 10));
		var game = new Game(new Board(4, 3), [], [player], 1, 2, 8);

		game.Step(0.5);
		Assert.Empty(game.Foods);
		game.Step(0.5);
		Assert.Equal(2, game.Foods.Count);
		game.Step(0.5);
		Assert.Equal(2, game.Foods.Count);
		game.Step(0.5);
		Assert.Equal(4, game.Foods.Count);
		Assert.Equal(4, game.Foods.Select(food => food.Position).Distinct().Count());
		Assert.All(game.Foods, food =>
		{
			Assert.True(game.Board.Contains(food.Position));
			Assert.DoesNotContain(food.Position, player.Snake.Body);
		});
	}

	[Fact]
	public void FoodSpawnStopsAtLimitEvenWhenBatchWouldOverflow()
	{
		var player = new Player("Player1", new Snake([new(0, 0)], Direction.Right, 10));
		var game = new Game(new Board(4, 3), [new Food(new(3, 2))], [player], 1, 2, 2);

		game.Step(1);
		Assert.Equal(2, game.Foods.Count);
		Position[] firstBatch = [.. game.Foods.Select(food => food.Position)];
		game.Step(1);

		Assert.Equal(firstBatch, game.Foods.Select(food => food.Position));
	}

	[Fact]
	public void FoodSpawnUsesLastFreeCellAndSkipsWhenBoardIsFull()
	{
		var player = new Player("Player1", new Snake([new(0, 0)], Direction.Right, 10));
		var game = new Game(new Board(3, 1), [new Food(new(1, 0))], [player], 1, 2, 8);

		game.Step(1);

		Assert.Equal(2, game.Foods.Count);
		Assert.Contains(game.Foods, food => food.Position.Equals(new Position(2, 0)));
		Assert.DoesNotContain(game.Foods, food => food.Position.Equals(new Position(0, 0)));
		game.Step(1);
		Assert.Equal(2, game.Foods.Count);
		Assert.True(player.IsAlive);
	}

	[Fact]
	public void FoodSpawnResumesAfterEatingCreatesCapacity()
	{
		var player = new Player("Player1", new Snake([new(0, 0)], Direction.Right, 0.25));
		var eaten = new Food(new Position(1, 0));
		var remaining = new Food(new Position(3, 1));
		var game = new Game(new Board(4, 2), [eaten, remaining], [player], 0.25, 2, 2);

		game.Step(0.25);

		Assert.Equal(3, player.Score);
		Assert.Equal<Position>([new(1, 0), new(0, 0)], player.Snake.Body);
		Assert.Equal(2, game.Foods.Count);
		Assert.DoesNotContain(eaten, game.Foods);
		Assert.Contains(remaining, game.Foods);
		Assert.Equal(2, game.Foods.Select(food => food.Position).Distinct().Count());
		Assert.All(game.Foods, food => Assert.DoesNotContain(food.Position, player.Snake.Body));
	}

	[Fact]
	public void GameCopiesInputListsButKeepsPlayerIdentity()
	{
		var player = new Player("Player1", new Snake([new(0, 0)], Direction.Right, 10));
		List<Player> players = [player];
		var food = new Food(new Position(2, 1));
		List<Food> foods = [food];
		var game = new Game(new Board(3, 2), foods, players, 1, 2, 8);

		players.Clear();
		foods.Clear();

		Assert.Same(player, Assert.Single(game.Players));
		Assert.Same(food, Assert.Single(game.Foods));
		Assert.True(game.RequestTurn(player, Direction.Down));
	}
}
