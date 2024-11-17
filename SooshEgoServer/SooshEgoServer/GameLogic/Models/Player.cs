using System.Text.Json.Serialization;

namespace SooshEgoServer.GameLogic.Models
{
    public class Player
    {
        [JsonPropertyName("name")]
        public PlayerName Name { get; init; }

        [JsonIgnore]
        public string? ConnectionId { get; set; }

        public Player(PlayerName name)
        {
            Name = name;
        }
    }

    public record PlayerName(string Value);
}
