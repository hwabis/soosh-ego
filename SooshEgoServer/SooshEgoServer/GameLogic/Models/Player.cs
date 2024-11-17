using System.Text.Json.Serialization;

namespace SooshEgoServer.GameLogic.Models
{
    public class Player
    {
        [JsonPropertyName("name")]
        public PlayerName Name { get; init; }

        [JsonIgnore]
        public string? ConnectionId { get; set; }

        // todo we're gonna make a list of cards in hand here and it's technically gonna be viewable by everyone
        // but i think it'll be too hard to hide it from certain players. so for now, just send all cards to everyone

        public Player(PlayerName name)
        {
            Name = name;
        }
    }

    public record PlayerName(string Value);
}
