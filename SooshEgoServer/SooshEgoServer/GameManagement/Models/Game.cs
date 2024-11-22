using System.Text.Json.Serialization;

namespace SooshEgoServer.GameManagement.Models
{
    public class Game(GameId gameId)
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; } = gameId;

        [JsonPropertyName("players")]
        public List<Player> Players { get; init; } = [];
    }

    public record GameId(string Value);
}
