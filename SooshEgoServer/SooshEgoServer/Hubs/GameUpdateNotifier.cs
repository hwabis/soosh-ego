using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameManagement;
using SooshEgoServer.GameManagement.Models;

namespace SooshEgoServer.Hubs
{
    public class GameUpdateNotifier(IHubContext<GameHub> hubContext)
    {
        public void NotifyGameStateUpdated(object? sender, GameStateUpdatedEventArgs e)
        {
            _ = NotifyGameStateUpdated(e.Game);
        }

        private async Task NotifyGameStateUpdated(Game game)
        {
            var connectionIds = game.Players
                .Where(player => player.ConnectionId != null)
                .Select(player => player.ConnectionId!);

            await hubContext.Clients.Clients(connectionIds).SendAsync("GameStateUpdated", game);
        }
    }
}
