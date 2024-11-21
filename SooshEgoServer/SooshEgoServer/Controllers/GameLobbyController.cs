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

        [HttpPost("join")]
        public IActionResult JoinGame([FromBody] JoinGameRequest request)
        {
            (bool success, string error) = gamesManager.AddPlayerToGame(request.GameId, request.PlayerName);

            if (!success)
            {
                return BadRequest(error);
            }

            return Ok();
        }

        public record JoinGameRequest(GameId GameId, PlayerName PlayerName);
    }
}
