using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Models;

namespace SooshEgoServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private readonly HashSet<GameLobby> lobbies = [];

        [HttpGet]
        public Guid Get()
        {
            GameLobby newLobby = new();
            lobbies.Add(newLobby);

            return newLobby.Id;
        }

        [HttpGet("{id}")]
        public IActionResult GetLobbyById(Guid id)
        {
            GameLobby? lobby = lobbies.FirstOrDefault(lobby => lobby.Id == id);

            if (lobby != null)
                return Ok();

            return NotFound();
        }
    }
}
