using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Game
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; }

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

        public Game(GameId gameId)
        {
            GameId = gameId;

            ResetDeck();
        }

        public void ResetDeck()
        {
            Deck.Clear();

            for (int i = 0; i < 14; i++)
            {
                Deck.Push(new Card(CardType.Tempura));
                Deck.Push(new Card(CardType.Sashimi));
                Deck.Push(new Card(CardType.Dumpling));
            }

            for (int i = 0; i < 12; i++)
            {
                Deck.Push(new Card(CardType.MakiRoll2));
            }

            for (int i = 0; i < 8; i++)
            {
                Deck.Push(new Card(CardType.MakiRoll3));
            }

            for (int i = 0; i < 6; i++)
            {
                Deck.Push(new Card(CardType.MakiRoll1));
            }

            for (int i = 0; i < 10; i++)
            {
                Deck.Push(new Card(CardType.SalmonNigiri));
            }

            for (int i = 0; i < 5; i++)
            {
                Deck.Push(new Card(CardType.SquidNigiri));
            }

            for (int i = 0; i < 5; i++)
            {
                Deck.Push(new Card(CardType.EggNigiri));
            }

            for (int i = 0; i < 10; i++)
            {
                Deck.Push(new Card(CardType.Pudding));
            }

            for (int i = 0; i < 6; i++)
            {
                Deck.Push(new Card(CardType.Wasabi));
            }

            for (int i = 0; i < 4; i++)
            {
                Deck.Push(new Card(CardType.Chopsticks));
            }

            Card[] cards = [.. Deck];
            Deck.Clear();

            foreach (Card card in cards.OrderBy(_ => Random.Shared.Next()))
            {
                Deck.Push(card);
            }
        }
    }

    public enum GameStage
    {
        Waiting,
        Playing,
        Finished,
    }

    public record GameId(string Value);
}
