using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Player(PlayerName name)
    {
        [JsonPropertyName("name")]
        public PlayerName Name { get; init; } = name;

        [JsonPropertyName("cardsInHand")]
        public List<Card> CardsInHand { get; set; } = [];

        [JsonPropertyName("cardsInPlay")]
        public List<Card> CardsInPlay { get; init; } = [];

        [JsonPropertyName("finishedTurn")]
        public bool FinishedTurn { get; set; } = false;

        [JsonPropertyName("pointsAtEndOfPreviousRound")]
        public int PointsAtEndOfPreviousRound { get; set; } = 0;

        // todo someone could technically get the game state to send before everyone has made their move by reconnecting.
        // make a field ignored by the json to track the player's "queued move" or something

        [JsonPropertyName("connectionId")]
        public string? ConnectionId { get; set; }
    }

    public record PlayerName(string Value);
}
