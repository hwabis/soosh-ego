using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.GameManagement;
using SooshEgoServer.GameManagement.Models;

namespace SooshEgoServer.Controllers
{
    // todo controller-level unit test?
    [ApiController]
    [Route("api/gamelobby")]
    public class GameLobbyController(IGamesManager gamesManager) : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult CreateAndAddPlayerToGame([FromBody] PlayerName playerName)
        {
            (bool success, GameId? gameId, string error) = gamesManager.CreateAndAddPlayerToGame(playerName);

            if (!success)
            {
                return BadRequest(error);
            }

            if (gameId == null)
            {
                throw new Exception();
            }

            return Ok(gameId);
        }

        [HttpPost("add-player")]
        public IActionResult AddPlayerToGame([FromBody] JoinGameRequest request)
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
