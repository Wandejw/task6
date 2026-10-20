namespace Game.Core;

public class Player(string name, Snake snake)
{
	public string Name { get; } = name;
	public int Score { get; private set; } = 0;
	public Snake Snake { get; } = snake;
	public bool IsAlive { get; private set; } = true;

	public void IncreaseScore(int amount) => Score += amount;
	public void Die() => IsAlive = false;
}
