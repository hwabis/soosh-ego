using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Game(GameId gameId)
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; } = gameId;

        [JsonPropertyName("gameStage")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameStage GameStage { get; set; } = GameStage.Waiting;

        [JsonPropertyName("numberOfRoundsCompleted")]
        public int NumberOfRoundsCompleted { get; set; } = 0;

        [JsonPropertyName("players")]
        public List<Player> Players { get; init; } = [];

        [JsonPropertyName("winnerName")]
        public string? WinnerName { get; set; } = null;

        [JsonIgnore]
        public Stack<Card> Deck { get; private set; } = [];
    }

    public enum GameStage
    {
        Waiting,
        Playing,
        Finished,
    }

    public record GameId(string Value);
}
