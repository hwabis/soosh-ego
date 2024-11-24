using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Player(PlayerName name)
    {
        [JsonPropertyName("name")]
        public PlayerName Name { get; init; } = name;

        [JsonIgnore]
        public string? ConnectionId { get; set; }
    }

    public record PlayerName(string Value);
}
