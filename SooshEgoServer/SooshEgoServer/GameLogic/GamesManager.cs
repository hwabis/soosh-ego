namespace SooshEgoServer.GameLogic
{
    public class GamesManager : IGamesManager
    {
        private readonly ILogger<GamesManager> logger;

        private List<Game> games = [];
        private int gameIdCounter = 0; // todo come up with something cooler

        public GamesManager(ILogger<GamesManager> logger)
        {
            this.logger = logger;
        }

        public GameId CreateGame()
        {
            // todo thread-safe?
            GameId newId = new(gameIdCounter.ToString());
            Game newGame = new(newId);
            games.Add(newGame);

            gameIdCounter++;

            return newId;
        }

        public bool AddPlayerToGame(GameId gameId, PlayerName playerName)
        {
            // todo 5-person check
            Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

            if (matchingGame != null)
            {
                if (matchingGame.Players.Any(player => player.Name == playerName))
                {
                    return false;
                }

                matchingGame.Players.Add(new Player(playerName));
                return true;
            }

            return false;
        }

        public void OnPlayerJoin(GameId gameId, PlayerName playerName, string connectionId)
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
                }
            }
            else
            {
                logger.LogError("{PlayerName} tried to join {GameId}, but the game did not exist", playerName, gameId);
            }
        }

        public void OnPlayerLeave(string connectionId)
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
                logger.LogError("A player disconnected while not in a game");
            }
        }
    }
}
