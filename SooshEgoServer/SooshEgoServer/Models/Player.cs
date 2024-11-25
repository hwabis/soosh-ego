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
        public List<Card> CardsInPlay { get; set; } = [];

        [JsonPropertyName("finishedTurn")]
        public bool FinishedTurn { get; set; } = false;

        [JsonPropertyName("pointsAtEndOfPreviousRound")]
        public int PointsAtEndOfPreviousRound { get; set; } = 0;

        [JsonPropertyName("connectionId")]
        public string? ConnectionId { get; set; }

        /// <summary>
        /// Serves as a buffer when moving a card from hand to in-play, so that the move is not immediately revealed to all players
        /// until everybody has made their move. So this only ever has at most 2 cards (1 on a normal play, 2 for chopsticks).
        /// </summary>
        [JsonIgnore]
        public List<Card> EnqueuedCardsToPlay { get; set; } = [];
    }

    public record PlayerName(string Value);
}
