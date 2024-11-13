using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.Models;
using System.Collections.Concurrent;

namespace SooshEgoServer.Hubs
{
    public class GameHub : Hub
    {
        // Lobby == group of players who aren't in-game yet.
        // The first player in each list is the lobby leader
        private static readonly ConcurrentDictionary<Guid, List<Player>> lobbies = [];
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> lobbyLocks = [];

        public async Task<Guid> Lobby_CreateLobby(string playerName)
        {
            var newLobbyId = Guid.NewGuid();

            if (!lobbies.TryAdd(newLobbyId, []))
            {
                throw new Exception("Failed to create a new lobby.");
            }

            if (!lobbyLocks.TryAdd(newLobbyId, new SemaphoreSlim(1, 1)))
            {
                throw new Exception("Failed to create a lobby lock.");
            }

            await addPlayerToLobbyAsync(newLobbyId, playerName);

            return newLobbyId;
        }

        public async Task<bool> Lobby_JoinLobby(Guid lobbyId, string playerName)
        {
            if (!lobbies.TryGetValue(lobbyId, out var lobbyPlayers))
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
                if (lobbyPlayers.Count >= 5)
                {
                    return false;
                }

                await addPlayerToLobbyAsync(lobbyId, playerName);

                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task addPlayerToLobbyAsync(Guid lobbyId, string playerName)
        {
            lobbies[lobbyId].Add(new Player(Context.ConnectionId, playerName));
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }
    }
}
