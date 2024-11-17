namespace SooshEgoServer.GameLogic
{
    public class Game // todo is this serializable?
    {
        public GameId GameId { get; init; }
        public List<Player> Players { get; init; } = [];

        public Game(GameId gameId)
        {
            GameId = gameId;
        }
    }

    public record GameId(string Value);
}
