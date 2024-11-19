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

            return Ok(gameId);
        }

        [HttpPost("{gameId}/join")]
        public IActionResult JoinGame([FromRoute] string gameId, [FromBody] string playerName)
        {
            (bool success, string error) = gamesManager.AddPlayerToGame(new(gameId), new(playerName));

            if (!success)
            {
                logger.LogInformation("{PlayerName} could not join {GameId} - {Error}", playerName, gameId, error);
                return BadRequest(error);
            }

            logger.LogInformation("{PlayerName} joined {GameId}", playerName, gameId);
            return Ok();
        }
    }
}
