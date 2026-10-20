namespace Game.Core.Tests;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var board = new Board(4, 3);
		var position = new Position(3, 2);
		bool result = board.Contains(position);
		Assert.True(result);
	}
	[Fact]
	public void Test2()
	{
		var board = new Board(4, 3);
		var position = new Position(4, 2);
		bool result = board.Contains(position);
		Assert.False(result);
	}
	[Fact]
	public void Test3()
	{
		var board = new Board(4, 3);
		var position = new Position(-1, 2);
		bool result = board.Contains(position);
		Assert.False(result);
	}
	[Fact]
	public void FoodStorePosition()
	{
		var position = new Position(1, 2);
		var food = new Food(position);
		Assert.Equal(position, food.Position);
	}
	[Fact]
	public void SnakeStoreBody()
	{
		List<Position> initialBody = [new Position(1, 2), new Position(1, 3)];
		var initialDirection = Direction.Up;
		var snake = new Snake(initialBody, initialDirection, 0.2);
		Assert.Equal(initialBody, snake.Body);
		Assert.Equal(initialDirection, snake.CurrentDirection);
	}
	[Fact]
	public void SnakeAdvanceTime()
	{
		var snake = new Snake([new Position(1, 2)], Direction.Up, 0.2);
		bool result1 = snake.AdvanceTime(0.25);
		Assert.True(result1);
		var position = snake.GetNextPosition();
		Assert.Equal(new Position(1, 1), position);
		snake.Move(position);
		Assert.Equal(new Position(1, 1), snake.Body[0]);
	}
	[Fact]
	public void GameStepEatMoveThenHitWall()
	{
		var board = new Board(4, 3);
		var players = new List<Player>();
		var player1 = new Player("Player1", new Snake([new Position(1, 1), new Position(0, 1)], Direction.Right, 0.25));
		players.Add(player1);
		var food = new Food(new Position(2, 1));
		List<Food> foodlist = [];
		foodlist.Add(food);
		var game = new Game(board, foodlist, players, 1.0, 2, 5);
		game.Step(0.25);
		Assert.Equal(3, player1.Score);
		Assert.Equal(new Position(2, 1), player1.Snake.Body[0]);
		Assert.Empty(game.Foods);
		Assert.Equal([new Position(2, 1), new Position(1, 1), new Position(0, 1)], player1.Snake.Body);
		game.Step(0.25);
		Assert.Equal(new Position(3, 1), player1.Snake.Body[0]);
		Assert.True(player1.IsAlive);
		game.Step(0.25);
		Assert.False(player1.IsAlive);
		Assert.True(game.IsOver);
	}
}
