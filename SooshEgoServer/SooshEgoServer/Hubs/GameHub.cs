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
    }
}
