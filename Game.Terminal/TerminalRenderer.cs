using Game.Core;
namespace Game.Terminal;

public class TerminalRenderer
{
	public static void DrawBoard(Core.Game game)
	{
		int width = game.Board.Width;
		int height = game.Board.Height;
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
					_ when game.Players.Count > 0 && game.Players[0].Snake.Body[0].Equals(position) => "@ ",
					_ when game.Players.Count > 0 && game.Players[0].Snake.Body.Contains(position) => "o ",
					_ when game.Foods.Any(food => food.Position.Equals(position)) => "* ",
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

	public static void DrawInfo(Core.Game game)
	{
		Console.WriteLine("操作：方向键移动");
		if (game.Players.Count > 0)
		{
			Console.WriteLine($"Score: {game.Players[0].Score}");
		}
	}

	public static void DrawGameOver(Core.Game game)
	{
		Console.WriteLine("Game Over!");
		if (game.Players.Count > 0)
		{
			Console.WriteLine($"Final Score: {game.Players[0].Score}");
		}

		Console.WriteLine("按A重新开始,按ESC退出");
	}
}
