using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Controllers.RequestBodies;
using SooshEgoServer.GameLogic;
using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private readonly ILogger<GameLobbyController> logger;
        private readonly IGamesManager gamesManager;

        public GameLobbyController(ILogger<GameLobbyController> logger, IGamesManager gamesManager)
        {
            this.logger = logger;
            this.gamesManager = gamesManager;
        }

        [HttpPost("create")]
        public IActionResult CreateGame()
        {
            GameId gameId = gamesManager.CreateGame();

            logger.LogInformation("Created {GameId}", gameId);
            return CreatedAtAction("", gameId);
        }

        [HttpPost("join")]
        public IActionResult JoinGame([FromBody] JoinGameRequest request)
        {
            (bool success, string error) = gamesManager.AddPlayerToGame(request.GameId, request.PlayerName);

            if (success)
            {
                logger.LogInformation("{PlayerName} joined {GameId}", request.PlayerName, request.GameId);
                return Ok();
            }
            else
            {
                logger.LogInformation("{PlayerName} could not join {GameId} - {Error}", request.PlayerName, request.GameId, error);
                return NotFound();
            }
        }
    }
}
