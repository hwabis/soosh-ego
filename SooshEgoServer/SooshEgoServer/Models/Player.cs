using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Player(PlayerName name)
    {
        [JsonPropertyName("name")]
        public PlayerName Name { get; init; } = name;

        [JsonPropertyName("cardsInHand")]
        public List<Card> CardsInHand { get; init; } = [];

        [JsonPropertyName("cardsInPlay")]
        public List<Card> CardsInPlay { get; init; } = [];

        [JsonPropertyName("connectionId")]
        public string? ConnectionId { get; set; }
    }

    public record PlayerName(string Value);
}
