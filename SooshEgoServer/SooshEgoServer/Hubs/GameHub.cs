using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameManagement;

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

        public override async Task OnConnectedAsync()
        {
            string? gameId = Context.GetHttpContext()?.Request.Query["gameId"];
            string? playerName = Context.GetHttpContext()?.Request.Query["playerName"];

            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(playerName))
            {
                await Clients.Caller.SendAsync("Error", "Missing gameId or playerName");
                return;
            }

            gamesManager.MarkPlayerConnected(new(gameId), new(playerName), Context.ConnectionId);

            await base.OnConnectedAsync();
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
