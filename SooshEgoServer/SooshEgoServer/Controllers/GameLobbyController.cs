using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Controllers.RequestBodies;
using SooshEgoServer.GameLogic;

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
            GameId? gameId = gamesManager.CreateGame();

            if (gameId != null)
            {
                logger.LogInformation("Created {GameId}", gameId);
                return CreatedAtAction("", gameId);
            }
            else
            {
                logger.LogWarning("Could not create a game");
                return NotFound();
            }
        }

        [HttpPost("join")]
        public IActionResult JoinGame([FromBody] JoinGameRequest request)
        {
            bool joined = gamesManager.JoinGame(request.GameId, request.PlayerName);

            if (joined)
            {
                logger.LogInformation("{PlayerName} joined {GameId}", request.PlayerName, request.GameId);
                return Ok();
            }
            else
            {
                logger.LogInformation("{PlayerName} could not join {GameId}", request.PlayerName, request.GameId);
                return NotFound();
            }
        }
    }
}
