using System.Text.Json.Serialization;

namespace SooshEgoServer.Models
{
    public class Game
    {
        [JsonPropertyName("gameId")]
        public GameId GameId { get; init; }

        [JsonPropertyName("gameStage")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameStage GameStage { get; private set; } = GameStage.Waiting;

        [JsonPropertyName("numberOfRoundsCompleted")]
        public int NumberOfRoundsCompleted { get; private set; } = 0;

        [JsonPropertyName("players")]
        public List<Player> Players { get; private set; } = [];

        [JsonPropertyName("winnerName")]
        public string? WinnerName { get; private set; } = null;

        [JsonIgnore]
        public Stack<Card> Deck { get; private set; } = [];

        public Game(GameId gameId)
        {
            GameId = gameId;

            ResetDeck();
        }

        public void StartNewGame()
        {
            GameStage = GameStage.Playing;
            NumberOfRoundsCompleted = 0;
            WinnerName = null;

            ResetDeck();

            foreach (Player player in Players)
            {
                player.CardsInPlay.Clear();
                player.CardsInHand.Clear();
                player.PointsAtEndOfPreviousRound = 0;
            }

            DistributeCardsFromDeckToPlayers();
        }

        /// <summary>
        /// Do not call for the first round of the game; only call <see cref="StartNewGame"/> instead.
        /// </summary>
        public void StartNewRound()
        {
            GameStage = GameStage.Playing;
            WinnerName = null;

            foreach (Player player in Players)
            {
                player.CardsInPlay.RemoveAll(card => card.CardType != CardType.Pudding);
                player.CardsInHand.Clear();
            }

            DistributeCardsFromDeckToPlayers();
        }

        public void OnGameEnd()
        {
            GameStage = GameStage.Finished;
            WinnerName = Players.MaxBy(player => player.PointsAtEndOfPreviousRound)?.Name.Value; // todo Add tiebreaker logic
        }

        /// <summary>
        /// Needs to always be called on the end of a round, even after the last round of the game.
        /// </summary>
        public void OnRoundEnd()
        {
            GameStage = GameStage.Waiting;
            NumberOfRoundsCompleted++;

            // todo calculate points
        }

        /// <summary>
        /// Needs to always be called on the end of a turn, even after the last turn of a round.
        /// </summary>
        public void OnTurnEnd()
        {
            foreach (Player player in Players)
            {
                player.FinishedTurn = false;
                player.CardsInPlay.AddRange(player.EnqueuedCardsToPlay);
                player.EnqueuedCardsToPlay.Clear();
            }

            RotatePlayerHands();
        }

        private void ResetDeck()
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

        private void DistributeCardsFromDeckToPlayers()
        {
            int numberOfCardsInStartingHand = Players.Count switch
            {
                2 => 10,
                3 => 9,
                4 => 8,
                5 => 7,
                _ => throw new Exception("Started a game with an invalid number of players."),
            };

            foreach (Player player in Players)
            {
                for (int i = 0; i < numberOfCardsInStartingHand; i++)
                {
                    DrawCard(player);
                }
            }

            return;
        }

        private void DrawCard(Player playerDrawingCard)
        {
            if (!Deck.TryPop(out Card? drawnCard))
            {
                return;
            }

            playerDrawingCard.CardsInHand.Add(drawnCard);

            return;
        }

        private void RotatePlayerHands()
        {
            List<Card> lastPlayerHand = Players[^1].CardsInHand;

            for (int i = Players.Count - 1; i > 0; i--)
            {
                Players[i].CardsInHand = Players[i - 1].CardsInHand;
            }

            Players[0].CardsInHand = lastPlayerHand;
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
