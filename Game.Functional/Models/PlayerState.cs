namespace Game.Functional.Models;

class PlayerState(SnakeState snake, Direction? pendingDirection, double moveElapsedSeconds)
{
	public SnakeState Snake { get; } = snake;
	public Direction? PendingDirection { get; } = pendingDirection;
	public double MoveElapsedSeconds { get; } = moveElapsedSeconds;
}
