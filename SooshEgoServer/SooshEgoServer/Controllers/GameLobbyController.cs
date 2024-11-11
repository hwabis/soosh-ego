using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Models;

namespace SooshEgoServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private static readonly HashSet<GameLobby> lobbies = [];

        [HttpPost("create")]
        public IActionResult CreateLobby()
        {
            GameLobby newLobby = new();
            lobbies.Add(newLobby);

            return Created("", newLobby.Id);
        }

        [HttpPost("join")]
        public IActionResult JoinLobbyById([FromBody] JoinLobbyRequest request)
        {
            GameLobby? lobby = lobbies.FirstOrDefault(lobby => lobby.Id == request.LobbyId);

            if (lobby == null)
            {
                return NotFound("Lobby does not exist.");
            }

            if (lobby.Players.Count == 5) // >= ?
            {
                // todo: i dont think this accounts for if multiple players join at the same time
                // and the lobby limit exceeds while the websocket connections are being made...

                return Conflict("Lobby is full.");
            }

            return Ok();
        }
    }
}
