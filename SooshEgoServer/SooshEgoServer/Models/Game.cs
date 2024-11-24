using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Game(GameId gameId)
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; } = gameId;

        [JsonPropertyName("gameState")]
        public GameStage GameStage { get; set; } = GameStage.Lobby;

        [JsonPropertyName("players")]
        public List<Player> Players { get; init; } = [];
    }

    public enum GameStage
    {
        Lobby,
        Round1,
        Round2,
        Round3,
        Finished,
    }

    public record GameId(string Value);
}
