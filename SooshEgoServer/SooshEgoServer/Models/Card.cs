using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Card(CardType cardType)
    {
        [JsonPropertyName("cardType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType CardType { get; init; } = cardType;
    }

    public enum CardType
    {
        Tempura,
        Sashimi,
        Dumpling,
        MakiRoll1,
        MakiRoll2,
        MakiRoll3,
        SalmonNigiri,
        SquidNigiri,
        EggNigiri,
        Wasabi,
        Pudding,
        Chopsticks,
    }
}
