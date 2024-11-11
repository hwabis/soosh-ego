using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Models;
using System.Collections.Concurrent;

namespace SooshEgoServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, GameLobby> lobbies = [];

        private readonly ILogger<GameLobbyController> logger;

        public GameLobbyController(ILogger<GameLobbyController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("create")]
        public IActionResult CreateLobby()
        {
            Guid newGuid = Guid.NewGuid();

            if (!lobbies.TryAdd(newGuid, new GameLobby()))
            {
                logger.LogError($"Failed to create a new lobby with ID {newGuid}");
                throw new Exception("Failed to create a new lobby.");
            }

            logger.LogInformation($"Created new lobby with ID {newGuid}");

            return Created("", new { LobbyId = newGuid });
        }

        [HttpPost("join")]
        public IActionResult JoinLobbyById([FromBody] JoinLobbyRequest request)
        {
            if (!lobbies.TryGetValue(request.LobbyId, out var lobby))
            {
                return NotFound("Lobby not found.");
            }

            lock (lobby)
            {
                if (lobby.Players.Count > 5)
                {
                    throw new Exception($"Lobby {request.LobbyId} exceeded the maximum player limit. Current player count: {lobby.Players.Count}");
                }

                if (lobby.Players.Count == 5)
                {
                    return Conflict("Lobby is full.");
                }

                lobby.Players.Add(new Player(request.PlayerName));
            }

            logger.LogInformation($"Player {request.PlayerName} joined lobby {request.LobbyId}. Current player count: {lobby.Players.Count}");

            return Ok();
        }
    }
}
