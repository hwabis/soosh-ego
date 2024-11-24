using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.Models;
using SooshEgoServer.Services;
using Xunit;

namespace SooshEgoServer.Tests.Services
{
    public class GameLogicTests
    {
        private readonly Mock<ILogger<GamesManager>> mockLogger;
        private readonly GamesManager gamesManager;

        public GameLogicTests()
        {
            mockLogger = new Mock<ILogger<GamesManager>>();
            gamesManager = new GamesManager(mockLogger.Object);
        }

        // todo
    }
}
