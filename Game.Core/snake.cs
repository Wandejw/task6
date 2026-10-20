namespace Game.Core;

public class Snake
{
	readonly double moveIntervalSeconds;
	double moveElapsedSeconds = 0;

	readonly List<Position> body;

	public Direction CurrentDirection { get; private set; }

	public Snake(List<Position> initialBody, Direction initialDirection, double intervalSeconds)
	{
		body = [.. initialBody];
		CurrentDirection = initialDirection;
		moveIntervalSeconds = intervalSeconds;
		Body = body.AsReadOnly();
	}

	public bool AdvanceTime(double elapsedSeconds)
	{
		moveElapsedSeconds += elapsedSeconds;
		if (moveElapsedSeconds >= moveIntervalSeconds)
		{
			moveElapsedSeconds -= moveIntervalSeconds;
			return true;
		}

		return false;
	}

	public IReadOnlyList<Position> Body { get; }

	public Position GetNextPosition()
	{
		var head = body[0];
		var newDirection = pendingDirection ?? CurrentDirection;

		return newDirection switch
		{
			Direction.Up => new Position(head.X, head.Y - 1),
			Direction.Down => new Position(head.X, head.Y + 1),
			Direction.Left => new Position(head.X - 1, head.Y),
			Direction.Right => new Position(head.X + 1, head.Y),
			_ => throw new InvalidOperationException("Invalid direction"),
		};
	}

	public void Move(Position nextPosition)
	{
		body.Insert(0, nextPosition);
		body.RemoveAt(body.Count - 1);
		if (pendingDirection.HasValue)
		{
			CurrentDirection = pendingDirection.Value;
			pendingDirection = null;
		}
	}

	public void Grow(Position nextPosition)
	{
		body.Insert(0, nextPosition);
		if (pendingDirection.HasValue)
		{
			CurrentDirection = pendingDirection.Value;
			pendingDirection = null;
		}
	}

	Direction? pendingDirection = null;

	public bool RequestTurn(Direction newDirection)
	{
		if (pendingDirection.HasValue)
		{
			return false;
		}

		if (newDirection == CurrentDirection)
		{
			return false;
		}

		if ((CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
			(CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
			(CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
			(CurrentDirection == Direction.Right && newDirection == Direction.Left))
		{
			return false;
		}

		pendingDirection = newDirection;
		return true;
	}
}
