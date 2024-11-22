﻿using SooshEgoServer.GameManagement.Models;
using System.Text;

namespace SooshEgoServer.GameManagement
{
    public class GamesManager(ILogger<GamesManager> logger) : IGamesManager
    {
        private readonly Dictionary<GameId, Game> games = [];
        private const int gameIdLength = 6;
        private const int gamePlayerLimit = 5;

        private readonly object gamesLock = new();

        public event EventHandler<GameStateUpdatedEventArgs>? GameStateUpdated;

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
                else
                {
                    logger.LogInformation("Created {GameId}", newId);

                    return (true, newId, "");
                }
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
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    return (false, "There is no game with the specified game ID.");
                }

                if (matchingGame.Players.Count >= gamePlayerLimit)
                {
                    if (matchingGame.Players.Count > gamePlayerLimit)
                    {
                        throw new Exception($"There were more than {gamePlayerLimit} players in {gameId}");
                    }

                    return (false, "The game's player limit is full.");
                }

                if (matchingGame.Players.Any(player => player.Name == playerName))
                {
                    return (false, "That name is already taken.");
                }

                matchingGame.Players.Add(new Player(playerName));
                logger.LogInformation("{PlayerName} joined {GameId}", playerName, gameId);

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));

                return (true, "");
            }
        }

        public (bool success, Game? game) GetGameState(GameId gameId)
        {
            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    logger.LogWarning("Tried to get non-existent game {GameId}", gameId);
                    return (false, null);
                }

                return (true, matchingGame);
            }
        }

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId)
        {
            lock (gamesLock)
            {
                if (!games.TryGetValue(gameId, out Game? matchingGame))
                {
                    throw new Exception($"{playerName} tried to join {gameId}, but the game did not exist\"");
                }

                Player? player = matchingGame.Players.FirstOrDefault(player => player.Name == playerName)
                    ?? throw new Exception($"{playerName} tried to join {gameId}, but the player did not exist in the game");

                if (player.ConnectionId != null)
                {
                    throw new Exception($"{playerName} tried to join {gameId}, but that player was already marked as connected");
                }

                player.ConnectionId = connectionId;
                logger.LogInformation("{PlayerName} connected to {GameId}", playerName, gameId);

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
            }
        }

        public void MarkPlayerDisconnectedAndCleanup(string connectionId)
        {
            lock (gamesLock)
            {
                Game? matchingGame = games.Values
                    .FirstOrDefault(game => game.Players.Any(player => player.ConnectionId == connectionId));

                if (matchingGame == null)
                {
                    logger.LogWarning("Disconnect received for connection {ConnectionId} which was not in a game", connectionId);
                    return;
                }

                Player? matchingPlayer = matchingGame.Players
                    .FirstOrDefault(player => player.ConnectionId == connectionId);

                if (matchingPlayer == null)
                {
                    logger.LogWarning("Disconnect received for unknown connection {ConnectionId}", connectionId);
                    return;
                }

                matchingPlayer.ConnectionId = null;
                logger.LogInformation("{PlayerName} disconnected from {GameId}", matchingPlayer.Name, matchingGame.GameId);

                if (matchingGame.Players.All(player => player.ConnectionId == null))
                {
                    games.Remove(matchingGame.GameId);
                    logger.LogInformation("Removed inactive game {GameId}", matchingGame.GameId);
                }

                GameStateUpdated?.Invoke(this, new GameStateUpdatedEventArgs(matchingGame));
            }
        }

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
    }
}
