using Microsoft.AspNetCore.Mvc;
using SooshEgoServer.Models;
using System.Collections.Concurrent;

namespace SooshEgoServer.Controllers
{
    // todo remove this whole thing
    [ApiController]
    [Route("[controller]")]
    public class GameLobbyController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, GameLobby> lobbies = [];
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> lobbyLocks = [];

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
                throw new Exception($"Failed to create a new lobby  with ID {newGuid}.");
            }

            logger.LogInformation($"Created new lobby with ID {newGuid}");

            return Created("", new { LobbyId = newGuid });
        }

        [HttpPost("join")]
        public IActionResult JoinLobbyById([FromBody] JoinLobbyRequest request)
        {
            if (!lobbies.TryGetValue(request.LobbyId, out var lobby))
            {
                logger.LogInformation($"Lobby {request.LobbyId} not found.");
                return NotFound($"Lobby not found.");
            }

            lock (lobby)
            {
                if (lobby.Players.Count > 5)
                {
                    throw new Exception($"Lobby {request.LobbyId} exceeded the maximum player limit. Current player count: {lobby.Players.Count}");
                }

                if (lobby.Players.Count == 5)
                {
                    logger.LogInformation($"Player {request.PlayerName} attempted to joined lobby {request.LobbyId}, but the lobby is full.");
                    return Conflict("Lobby is full.");
                }

                lobby.Players.Add(new Player(request.PlayerName, "weiofiwoefj"));
            }

            logger.LogInformation($"Player {request.PlayerName} joined lobby {request.LobbyId}. Current player count: {lobby.Players.Count}");

            return Ok();
        }
    }
}
