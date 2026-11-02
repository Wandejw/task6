using Game.Functional.Models;

namespace Game.Functional.Logic;

static class Collision
{
	public static bool IsCollision(SnakeState snake, int width, int height)
		=> snake.Head.X >= width
		|| snake.Head.Y >= height
		|| snake.Head.X < 0
		|| snake.Head.Y < 0
		|| snake.Body.Skip(1).Contains(snake.Head);
}
