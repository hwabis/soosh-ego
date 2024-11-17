using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameLogic;

namespace SooshEgoServer.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGamesManager gamesManager;

        public GameHub(IGamesManager gamesManager)
        {
            this.gamesManager = gamesManager;
        }

        public async Task JoinGame(GameId gameId, PlayerName playerName)
        {
            gamesManager.MarkPlayerConnected(gameId, playerName, Context.ConnectionId);
            /*
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.Value);
            
            IEnumerable<PlayerName>? namesList = gamesManager.GetPlayerNames(gameId);
            await Clients.Group(gameId.Id).SendAsync("UpdatePlayerList", playerList);todo yo come back*/
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gamesManager.MarkPlayerDisconnected(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
