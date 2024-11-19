using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.GameLogic;
using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.Controllers
{
    // todo controller-level unit test?
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private readonly IGamesManager gamesManager;

        public GameLobbyController(IGamesManager gamesManager)
        {
            this.gamesManager = gamesManager;
        }

        [HttpPost("create")]
        public IActionResult CreateGame()
        {
            GameId gameId = gamesManager.CreateGame();

            return Ok(gameId);
        }

        [HttpPost("{gameId}/join")]
        public IActionResult JoinGame([FromRoute] string gameId, [FromBody] string playerName)
        {
            (bool success, string error) = gamesManager.AddPlayerToGame(new(gameId), new(playerName));

            if (!success)
            {
                return BadRequest(error);
            }

            return Ok();
        }
    }
}
