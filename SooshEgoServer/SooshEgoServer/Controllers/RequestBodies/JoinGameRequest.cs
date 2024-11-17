using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.Controllers.RequestBodies
{
    public class JoinGameRequest
    {
        public required GameId GameId { get; set; }
        public required PlayerName PlayerName { get; set; }
    }
}
