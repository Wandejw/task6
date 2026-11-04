namespace Game.Functional.Models;

public record PlayerState(SnakeState Snake, Direction? PendingDirection, double MoveElapsedSeconds);
