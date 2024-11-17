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

        public void JoinGame(GameId gameId, PlayerName playerName)
        {
            gamesManager.OnPlayerJoin(gameId, playerName, Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gamesManager.OnPlayerLeave(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
