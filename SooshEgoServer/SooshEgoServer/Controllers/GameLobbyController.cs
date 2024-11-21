using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.GameLogic;
using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.Controllers
{
    // todo controller-level unit test?
    [ApiController]
    [Route("api/gamelobby")]
    public class GameLobbyController(IGamesManager gamesManager) : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult CreateAndJoinGame([FromBody] PlayerName playerName)
        {
            GameId gameId = gamesManager.CreateGame();

            (bool success, string error) = gamesManager.AddPlayerToGame(gameId, playerName);

            if (!success)
            {
                gamesManager.DeleteGame(gameId);
                return BadRequest(error);
            }

            return Ok(gameId);
        }

        [HttpPost("join")]
        public IActionResult JoinExistingGame([FromBody] JoinGameRequest request)
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
