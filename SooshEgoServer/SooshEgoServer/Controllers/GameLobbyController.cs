using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Models;
using SooshEgoServer.Services;

namespace SooshEgoServer.Controllers
{
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

        [HttpPost("start-game")]
        public IActionResult StartGame([FromBody] GameId gameId)
        {
            (bool success, string error) = gamesManager.StartGame(gameId);

            if (!success)
            {
                return BadRequest(error);
            }

            return Ok();
        }
    }
}
