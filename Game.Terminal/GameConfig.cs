using Game.Functional.Models;

namespace Game.Terminal;

public record GameConfig(int Width, int Height, GameRules Rules);
