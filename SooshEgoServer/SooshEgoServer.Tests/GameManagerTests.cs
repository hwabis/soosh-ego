using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.GameLogic;
using Xunit;

namespace SooshEgoServer.Tests
{
    public class GameManagerTests
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

        [Fact]
        public void TestAddPlayers()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            GameId gameId = gamesManager.CreateGame();

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("")));
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("1")));
            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("1")));
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("2")));
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("3")));
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("4")));
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("5")));
            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("6")));

            Assert.False(gamesManager.AddPlayerToGame(new GameId("mystery lobby"), new PlayerName("mr. lost")));
        }
    }
}