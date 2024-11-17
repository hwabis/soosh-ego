using System.Text.Json.Serialization;

namespace SooshEgoServer.GameLogic.Models
{
    public class Game
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; }

        [JsonPropertyName("players")]
        public List<Player> Players { get; init; } = [];

        public Game(GameId gameId)
        {
            GameId = gameId;
        }
    }

    public record GameId(string Value);
}
