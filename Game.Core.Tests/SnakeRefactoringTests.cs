namespace Game.Core.Tests;

public class SnakeRefactoringTests
{
	[Fact]
	public void ConstructorCopiesBodyContainer()
	{
		List<Position> initialBody = [new(1, 1), new(0, 1)];
		var snake = new Snake(initialBody, Direction.Right, 0.25);

		initialBody.Clear();

		Assert.Equal<Position>([new(1, 1), new(0, 1)], snake.Body);
	}

	[Theory]
	[InlineData(Direction.Right)]
	[InlineData(Direction.Left)]
	public void IgnoredTurnLeavesRoomForValidTurn(Direction ignoredDirection)
	{
		var snake = new Snake([new(1, 1), new(0, 1)], Direction.Right, 0.25);

		Assert.False(snake.RequestTurn(ignoredDirection));
		Assert.True(snake.RequestTurn(Direction.Up));
		Assert.Equal(new Position(1, 0), snake.GetNextPosition());
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void MovementCommitsFirstTurnAndAllowsNextTurn(bool grow)
	{
		var snake = new Snake([new(1, 1), new(0, 1)], Direction.Right, 0.25);
		Assert.True(snake.RequestTurn(Direction.Up));
		Assert.False(snake.RequestTurn(Direction.Left));
		Assert.Equal(Direction.Right, snake.CurrentDirection);
		var nextPosition = snake.GetNextPosition();

		if (grow)
			snake.Grow(nextPosition);
		else
			snake.Move(nextPosition);

		Position[] expectedBody = grow
			? [new(1, 0), new(1, 1), new(0, 1)]
			: [new(1, 0), new(1, 1)];
		Assert.Equal(expectedBody, snake.Body);
		Assert.Equal(Direction.Up, snake.CurrentDirection);
		Assert.True(snake.RequestTurn(Direction.Left));
		Assert.Equal(new Position(0, 0), snake.GetNextPosition());
	}
}
