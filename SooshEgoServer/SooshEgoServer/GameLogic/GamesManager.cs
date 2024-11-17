﻿using System.Data;

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

        public (bool success, string errorMessage) AddPlayerToGame(GameId gameId, PlayerName playerName)
        {
            if (playerName.Value == string.Empty)
            {
                return (false, "Player name cannot be empty.");
            }

            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame == null)
                {
                    return (false, "There is no game with the specified game ID.");
                }

                if (matchingGame.Players.Count >= gamePlayerLimit)
                {
                    if (matchingGame.Players.Count > gamePlayerLimit)
                    {
                        logger.LogError("There were more than {GamePlayerLimit} players in {GameId}", gamePlayerLimit, gameId);
                        throw new Exception();
                    }

                    return (false, "The game's player limit is full.");
                }

                if (matchingGame.Players.Any(player => player.Name == playerName))
                {
                    return (false, "That name is already taken.");
                }

                matchingGame.Players.Add(new Player(playerName));

                return (true, "");
            }
        }

        public (bool success, IEnumerable<PlayerName> playerNames) GetPlayerNames(GameId gameId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame == null)
                {
                    logger.LogWarning("Tried to get player names of non-existent game {GameId}", gameId);
                    return (false, []);
                }

                return (true, matchingGame.Players.Select(player => player.Name));
            }
        }

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.FirstOrDefault(game => game.GameId == gameId);

                if (matchingGame == null)
                {
                    logger.LogError("{PlayerName} tried to join {GameId}, but the game did not exist", playerName, gameId);
                    throw new Exception();
                }

                Player? player = matchingGame.Players.FirstOrDefault(player => player.Name == playerName);

                if (player == null)
                {
                    logger.LogError("{PlayerName} tried to join {GameId}, but the player did not exist in the game", playerName, gameId);
                    throw new Exception();
                }

                player.ConnectionId = connectionId;
            }
        }

        public void MarkPlayerDisconnected(string connectionId)
        {
            lock (gamesLock)
            {
                // todo change games to dict so we don't have to linq
                Player? matchingPlayer = games
                    .SelectMany(game => game.Players)
                    .FirstOrDefault(player => player.ConnectionId == connectionId);

                if (matchingPlayer == null)
                {
                    logger.LogWarning("Disconnect received for unknown connection {ConnectionId}", connectionId);
                    return;
                }

                matchingPlayer.ConnectionId = null;
            }
        }
    }
}
