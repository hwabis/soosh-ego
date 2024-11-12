using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.Models;
using System.Collections.Concurrent;

namespace SooshEgoServer.Hubs
{
    public class GameLobbyHub : Hub
    {
        private static readonly ConcurrentDictionary<Guid, GameLobby> lobbies = [];
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> lobbyLocks = [];

        public async Task<Guid> CreateLobby(string playerName)
        {
            var newLobby = new GameLobby();
            var newLobbyId = Guid.NewGuid();

            if (!lobbies.TryAdd(newLobbyId, newLobby))
            {
                throw new Exception("Failed to create a new lobby.");
            }

            if (!lobbyLocks.TryAdd(newLobbyId, new SemaphoreSlim(1, 1)))
            {
                throw new Exception("Failed to create a lobby lock.");
            }

            await addPlayerToLobby(newLobbyId, playerName);

            return newLobbyId;
        }

        public async Task<bool> JoinLobby(Guid lobbyId, string playerName)
        {
            if (!lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            if (!lobbyLocks.TryGetValue(lobbyId, out var semaphore))
            {
                throw new Exception("Lobby exists but its lock does not exist.");
            }

            await semaphore.WaitAsync();

            try
            {
                if (lobby.Players.Count >= 5)
                {
                    return false;
                }

                await addPlayerToLobby(lobbyId, playerName);

                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task addPlayerToLobby(Guid lobbyId, string playerName)
        {
            GameLobby lobby = lobbies[lobbyId];
            lobby.Players.Add(new Player(Context.ConnectionId, playerName));
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }
    }
}
