namespace SooshEgoServer.GameLogic
{
    public class Game
    {
        public readonly GameId GameId;
        public readonly List<Player> Players = new();

        public Game(GameId gameId)
        {
            GameId = gameId;
        }
    }

    public record GameId(string Value);
}
