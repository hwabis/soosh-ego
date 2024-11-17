namespace SooshEgoServer.GameLogic
{
    public class GamesManager : IGamesManager
    {
        private readonly ILogger<GamesManager> logger;

        private List<Game> games = [];
        private readonly object gamesLock = new();
        private int gameIdCounter = 0; // todo come up with something cooler
        private const int gamePlayerLimit = 5;

        public GamesManager(ILogger<GamesManager> logger)
        {
            this.logger = logger;
        }

        public GameId CreateGame()
        {
            lock (gamesLock)
            {
                GameId newId = new(gameIdCounter.ToString());
                Game newGame = new(newId);
                games.Add(newGame);

                gameIdCounter++;

                return newId;
            }
        }

        public bool AddPlayerToGame(GameId gameId, PlayerName playerName)
        {
            if (playerName.Value == string.Empty)
            {
                return false;
            }

            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame != null)
                {
                    if (matchingGame.Players.Count >= gamePlayerLimit)
                    {
                        if (matchingGame.Players.Count > gamePlayerLimit)
                        {
                            logger.LogError("There were more than {GamePlayerLimit} players in {GameId}", gamePlayerLimit, gameId);
                            throw new Exception();
                        }

                        return false;
                    }

                    if (matchingGame.Players.Any(player => player.Name == playerName))
                    {
                        return false;
                    }

                    matchingGame.Players.Add(new Player(playerName));
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<PlayerName>? GetPlayerNames(GameId gameId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame != null)
                {
                    return matchingGame.Players.Select(player => player.Name);
                }
                else
                {
                    logger.LogError("Tried to get player names of non-existent game {GameId}", gameId);
                    return null;
                }
            }
        }

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame != null)
                {
                    Player? player = matchingGame.Players.FirstOrDefault(player => player.Name == playerName);

                    if (player != null)
                    {
                        player.ConnectionId = connectionId;
                    }
                    else
                    {
                        logger.LogError("{PlayerName} tried to join {GameId}, but the player did not exist in the game", playerName, gameId);
                        throw new Exception();
                    }
                }
                else
                {
                    logger.LogError("{PlayerName} tried to join {GameId}, but the game did not exist", playerName, gameId);
                    throw new Exception();
                }
            }
        }

        public void MarkPlayerDisconnected(string connectionId)
        {
            lock (gamesLock)
            {
                Player? matchingPlayer = null;

                foreach (Game game in games)
                {
                    // todo change games to dict so we don't have to linq
                    Player? potentiallyMatchingPlayer = game.Players.FirstOrDefault(player => player.ConnectionId == connectionId);

                    if (potentiallyMatchingPlayer != null)
                    {
                        matchingPlayer = potentiallyMatchingPlayer;
                    }
                }

                if (matchingPlayer != null)
                {
                    matchingPlayer.ConnectionId = null;
                }
                else
                {
                    logger.LogError("A player who was not in a game disconnected");
                    throw new Exception();
                }
            }
        }
    }
}
