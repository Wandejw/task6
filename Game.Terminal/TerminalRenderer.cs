using Game.Functional.Models;

namespace Game.Terminal;

public class TerminalRenderer
{
	public static void DrawBoard(WorldState world)
	{
		int width = world.Width;
		int height = world.Height;
		for (int x = 0; x < width + 2; x++)
		{
			Console.Write("##");
		}

		Console.WriteLine();
		for (int y = 0; y < height; y++)
		{
			Console.Write("##");
			for (int x = 0; x < width; x++)
			{
				var position = new Position(x, y);
				string symbol = position switch
				{
					_ when world.Player.Snake.Head == position => "@ ",
					_ when world.Player.Snake.Body.Contains(position) => "o ",
					_ when world.Foods.Contains(position) => "* ",
					_ => "  "
				};
				Console.Write(symbol);
			}

			Console.Write("##");
			Console.WriteLine();
		}

		for (int x = 0; x < width + 2; x++)
		{
			Console.Write("##");
		}

		Console.WriteLine();
	}

	public static void DrawInfo(WorldState world)
	{
		Console.WriteLine("操作：方向键移动");
		Console.WriteLine($"Length: {world.Player.Snake.Body.Length}");
	}

	public static void DrawGameOver(WorldState world)
	{
		Console.WriteLine("Game Over!");
		Console.WriteLine($"Final Length: {world.Player.Snake.Body.Length}");
		Console.WriteLine("按A重新开始,按ESC退出");
	}
}
