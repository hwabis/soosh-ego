using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameManagement;

namespace SooshEgoServer.Hubs
{
    // todo hub-level unit test?
    public class GameHub(IGamesManager gamesManager) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string? gameId = Context.GetHttpContext()?.Request.Query["gameId"];
            string? playerName = Context.GetHttpContext()?.Request.Query["playerName"];

            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(playerName))
            {
                await SendError("Missing game ID or player name");
                return;
            }

            (bool success, string error) = gamesManager.MarkPlayerConnected(new(gameId), new(playerName), Context.ConnectionId);

            if (!success)
            {
                await SendError(error);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            (bool success, string error) = gamesManager.MarkPlayerDisconnectedAndCleanup(Context.ConnectionId);

            if (!success)
            {
                await SendError(error);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("Error", message);
        }
    }
}
