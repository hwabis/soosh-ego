using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameManagement;
using SooshEgoServer.GameManagement.Models;

namespace SooshEgoServer.Hubs
{
    // todo hub-level unit test?
    public class GameHub : Hub
    {
        private readonly IGamesManager gamesManager;

        public GameHub(IGamesManager gamesManager)
        {
            this.gamesManager = gamesManager;
            this.gamesManager.GameStateUpdated += OnGameStateUpdated;
        }

        public void ConnectToGame(GameId gameId, PlayerName playerName)
        {
            gamesManager.MarkPlayerConnected(gameId, playerName, Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gamesManager.MarkPlayerDisconnectedAndCleanup(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        private async void OnGameStateUpdated(object? sender, GameStateUpdatedEventArgs e)
        {
            IEnumerable<string> connectionIds = e.Game.Players
                .Where(player => player.ConnectionId != null)
                .Select(player => player.ConnectionId!);

            await Clients.Clients(connectionIds).SendAsync("GameStateUpdated", e.Game); // todo client
        }
    }
}
