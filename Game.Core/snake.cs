namespace Game.Core;

public class Snake
{
	readonly double moveIntervalSeconds;
	double moveElapsedSeconds = 0;

	readonly List<Position> body;
	public Position Head => body[0];

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
	=> Head + (pendingDirection ?? CurrentDirection).Position;

	public void Grow(Position nextPosition)
	{
		body.Insert(0, nextPosition);
		if (pendingDirection is { } dir)
		{
			CurrentDirection = dir;
			pendingDirection = null;
		}
	}

	public void Move(Position nextPosition)
	{
		var newBody = CalculateMovedBody(body, nextPosition);
		body.Clear();
		body.AddRange(newBody);
		if (pendingDirection is { } dir)
		{
			CurrentDirection = dir;
			pendingDirection = null;
		}
	}

	Direction? pendingDirection = null;

	public bool RequestTurn(Direction newDirection)
	{
		bool isOpposite = newDirection == CurrentDirection.Opposite;

		if (pendingDirection.HasValue || newDirection == CurrentDirection || isOpposite)
		{
			return false;
		}

		pendingDirection = newDirection;
		return true;
	}

	public static List<Position> CalculateMovedBody(
	IReadOnlyList<Position> oldBody,
	Position nextPosition)
	{
		List<Position> newBody = [nextPosition, .. oldBody.Take(oldBody.Count - 1)];
		return newBody;
	}
}
