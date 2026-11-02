using System.Collections.Immutable;

using Game.Functional.Models;

namespace Game.Functional.Logic;

static class Movement
{
	public static SnakeState Move(SnakeState snake)
	{
		var newHead = snake.Head + snake.CurrentDirection.Position;
		ImmutableArray<Position> newBody = [newHead, .. snake.Body.Take(snake.Body.Length - 1)];
		var newDirection = snake.CurrentDirection;
		return new SnakeState(newBody, newDirection);
	}

	public static SnakeState Grow(SnakeState snake)
	{
		var newHead = snake.Head + snake.CurrentDirection.Position;
		ImmutableArray<Position> newBody = [newHead, .. snake.Body];
		var newDirection = snake.CurrentDirection;
		return new SnakeState(newBody, newDirection);
	}
}
