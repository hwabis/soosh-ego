using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.GameLogic;
using Xunit;

namespace SooshEgoServer.Tests
{
    public class TestGameManager
    {
        [Fact]
        public void TestCreatedGameIdsAreUnique()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            GameId gameId1 = gamesManager.CreateGame();
            GameId gameId2 = gamesManager.CreateGame();

            Assert.NotEqual(gameId1.Value, gameId2.Value);
        }
    }
}