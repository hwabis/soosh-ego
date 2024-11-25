using SooshEgoServer.Models;
using System.Text;

namespace SooshEgoServer.Services
{
    public class GamesManager(ILogger<GamesManager> logger) : IGamesManager
    {
        private readonly Dictionary<GameId, Game> games = [];
        private const int gameIdLength = 6;
        private const int gamePlayerLimit = 5;

        private readonly object gamesLock = new(); // todo revisit if i'm over-locking

        public event EventHandler<GameStateUpdatedEventArgs>? GameStateUpdated;

        #region General

        public (bool success, Game? game) GetGameState(GameId gameId)
        {
            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    logger.LogWarning("Attempted to get non-existent game {GameId}", gameId);
                    return (false, null);
                }

                return (true, matchingGame);
            }
        }

        public (bool success, string error) MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId)
        {
            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    logger.LogWarning("{PlayerName} tried to connect to {GameId}, but the game did not exist", playerName, gameId);
                    return (false, "There is no game with the specified game ID.");
                }

                Player? player = matchingGame.Players.FirstOrDefault(player => player.Name == playerName);

                if (player == null)
                {
                    logger.LogWarning("{PlayerName} tried to connect to {GameId}, but the player did not exist in the game", playerName, gameId);
                    return (false, "There is no player in the game with that name.");
                }

                if (player.ConnectionId != null)
                {
                    logger.LogWarning("{PlayerName} tried to connect to {GameId}, but that player was already marked as connected", playerName, gameId);
                    return (false, "A player with that name is already connected.");
                }

                player.ConnectionId = connectionId;
                logger.LogInformation("{PlayerName} connected to {GameId}", playerName, gameId);

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
                return (true, "");
            }
        }

        public (bool success, string error) MarkPlayerDisconnectedAndCleanup(string connectionId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.Values
                    .FirstOrDefault(game => game.Players.Any(player => player.ConnectionId == connectionId));

                if (matchingGame == null)
                {
                    logger.LogWarning("Disconnect received for connection {ConnectionId} which was not in a game", connectionId);
                    return (false, "Cannot disconnect while not in a game.");
                }

                Player? matchingPlayer = matchingGame.Players
                    .FirstOrDefault(player => player.ConnectionId == connectionId);

                if (matchingPlayer == null)
                {
                    logger.LogWarning("Disconnect received for unknown connection {ConnectionId}", connectionId);
                    return (false, "Cannot disconnect while not in a game.");
                }

                matchingPlayer.ConnectionId = null;
                logger.LogInformation("{PlayerName} disconnected from {GameId}", matchingPlayer.Name, matchingGame.GameId);

                if (matchingGame.Players.All(player => player.ConnectionId == null))
                {
                    games.Remove(matchingGame.GameId);
                    logger.LogInformation("Removed inactive game {GameId}", matchingGame.GameId);
                }

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
                return (true, "");
            }
        }

        #endregion

        #region Lobby

        public (bool success, GameId? gameId, string error) CreateAndAddPlayerToGame(PlayerName playerName)
        {
            lock (gamesLock)
            {
                GameId newId = CreateNewGameId();
                Game newGame = new(newId);
                games.Add(newId, newGame);

                (bool success, string errorMessage) = AddPlayerToGame(newId, playerName);

                if (!success)
                {
                    if (!games.Remove(newId))
                    {
                        throw new Exception($"Couldn't remove non-existant game {newId}");
                    }

                    return (false, null, errorMessage);
                }

                logger.LogInformation("Created {GameId}", newId);

                return (true, newId, "");
            }
        }

        public (bool success, string error) AddPlayerToGame(GameId gameId, PlayerName playerName)
        {
            if (playerName.Value == string.Empty)
            {
                return (false, "Player name cannot be empty.");
            }

            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    return (false, "There is no game with the specified game ID.");
                }

                if (matchingGame.GameStage != GameStage.Lobby)
                {
                    return (false, "The game has already started.");
                }

                if (matchingGame.Players.Count >= gamePlayerLimit)
                {
                    if (matchingGame.Players.Count > gamePlayerLimit)
                    {
                        throw new Exception($"There were more than {gamePlayerLimit} players in {gameId}");
                    }

                    return (false, "The game's lobby is full.");
                }

                if (matchingGame.Players.Any(player => player.Name == playerName))
                {
                    return (false, "The name is already taken.");
                }

                matchingGame.Players.Add(new Player(playerName));
                logger.LogInformation("{PlayerName} added to {GameId}", playerName, gameId);

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
                return (true, "");
            }
        }

        public (bool success, string error) StartGame(GameId gameId)
        {
            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    return (false, "There is no game with the specified game ID.");
                }

                if (matchingGame.GameStage != GameStage.Lobby)
                {
                    return (false, "The game is already in-progress.");
                }

                if (matchingGame.Players.Count < 2)
                {
                    return (false, "Requires at least two players to start.");
                }

                matchingGame.GameStage = GameStage.Round1;

                DistributeCardsFromDeck(matchingGame);

                logger.LogInformation("{GameId} has started", gameId);

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
                return (true, "");
            }
        }

        #endregion

        #region Gameplay

        public (bool success, string error) PlayCard(
            GameId gameId, PlayerName playerName, int indexOfCardInHand, int? indexOfSecondCardInHandUsingChopsticks)
        {
            if (indexOfCardInHand == indexOfSecondCardInHandUsingChopsticks)
            {
                logger.LogWarning("{PlayerName} in {GameId} tried to play the card at index {Index} twice", playerName, gameId, indexOfCardInHand);
                return (false, "Cannot play the same card twice in one turn.");
            }

            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    logger.LogWarning("{PlayerName} tried to play a card, but the game {GameId} did not exist", playerName, gameId);
                    return (false, "There is no game with the specified game ID.");
                }

                Player? player = matchingGame.Players.FirstOrDefault(player => player.Name == playerName);

                if (player == null)
                {
                    logger.LogWarning("{PlayerName} tried play a card, but the player did not exist in the game {GameId}", playerName, gameId);
                    return (false, "There is no player in the game with that name.");
                }

                if (player.FinishedTurn)
                {
                    logger.LogWarning("{PlayerName} in {GameId} tried play a second time on their turn", playerName, gameId);
                    return (false, "Player already went this turn.");
                }

                Card? card1 = TryGetCard(player, indexOfCardInHand);

                if (card1 == null)
                {
                    return (false, "Requested card index is out of bounds.");
                }

                Card? card2 = null;

                if (indexOfSecondCardInHandUsingChopsticks != null)
                {
                    card2 = TryGetCard(player, indexOfCardInHand);

                    if (card2 == null)
                    {
                        return (false, "Requested card index is out of bounds.");
                    }
                }

                player.CardsInPlay.Add(card1);
                player.CardsInHand.Remove(card1);
                logger.LogInformation("{PlayerName} in {GameId} played {Card1}", playerName, gameId, card1);

                if (card2 != null)
                {
                    player.CardsInPlay.Add(card2);
                    player.CardsInHand.Remove(card2);
                    logger.LogInformation("{PlayerName} in {GameId} played a second card {Card2}", playerName, gameId, card2);
                }

                player.FinishedTurn = true;

                if (matchingGame.Players.All(player => player.FinishedTurn))
                {
                    foreach (Player p in matchingGame.Players)
                    {
                        p.FinishedTurn = false;
                    }

                    bool roundEnded = matchingGame.Players.Any(player => player.CardsInHand.Count == 0);

                    if (roundEnded)
                    {
                        // todo go to gamestage waiting
                    }
                    else
                    {
                        RotatePlayerHands(matchingGame);
                    }

                    GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
                }

                return (true, "");
            }
        }

        #endregion

        private GameId CreateNewGameId()
        {
            const string charSet = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder result = new(gameIdLength);

            do
            {
                result.Clear();

                for (int i = 0; i < gameIdLength; i++)
                {
                    result.Append(charSet[Random.Shared.Next(charSet.Length)]);
                }
            } while (games.ContainsKey(new(result.ToString())));

            return new GameId(result.ToString());
        }

        private void DistributeCardsFromDeck(Game game)
        {
            var numberOfCardsInStartingHand = game.Players.Count switch
            {
                2 => 10,
                3 => 9,
                4 => 8,
                5 => 7,
                _ => throw new Exception("Started a game with an invalid number of players."),
            };

            foreach (Player player in game.Players)
            {
                for (int i = 0; i < numberOfCardsInStartingHand; i++)
                {
                    DrawCard(game, player);
                }
            }
        }

        private void DrawCard(Game game, Player playerDrawingCard)
        {
            if (!game.Players.Contains(playerDrawingCard))
            {
                throw new Exception("Player is not in the correct game.");
            }

            if (!game.Deck.TryPop(out Card? drawnCard))
            {
                logger.LogError("In game {GameId}, the deck is out of cards!", game.GameId);
                return;
            }

            playerDrawingCard.CardsInHand.Add(drawnCard);
        }

        private Card? TryGetCard(Player player, int index)
        {
            if (index < 0 || index >= player.CardsInHand.Count)
            {
                logger.LogWarning("{PlayerName} tried to play a card at index {Index}, but their hand has only {Count} cards",
                    player.Name, index, player.CardsInHand.Count);

                return null;
            }

            return player.CardsInHand[index];
        }

        private static void RotatePlayerHands(Game game)
        {
            List<Card> lastPlayerHand = game.Players[^1].CardsInHand;

            for (int i = game.Players.Count - 1; i > 0; i--)
            {
                game.Players[i].CardsInHand = game.Players[i - 1].CardsInHand;
            }

            game.Players[0].CardsInHand = lastPlayerHand;
        }
    }
}
