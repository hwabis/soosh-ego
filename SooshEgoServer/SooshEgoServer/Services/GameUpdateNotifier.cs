using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.Hubs;
using SooshEgoServer.Models;

namespace SooshEgoServer.Services
{
    public class GameUpdateNotifier(IHubContext<GameHub> hubContext)
    {
        public void NotifyGameStateUpdated(object? sender, GameStateUpdatedEventArgs e)
        {
            _ = NotifyGameStateUpdated(e.Game);
        }

        private async Task NotifyGameStateUpdated(Game game)
        {
            IEnumerable<string> connectionIds = game.Players
                .Where(player => player.ConnectionId != null)
                .Select(player => player.ConnectionId!);

            await hubContext.Clients.Clients(connectionIds).SendAsync("GameStateUpdated", game);
        }
    }
}
