using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.GameLogic;
using SooshEgoServer.GameLogic.Models;
using Xunit;

namespace SooshEgoServer.Tests
{
    public class GamesManagerTests
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
        public void TestAddAndGetPlayers()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            GameId gameId = gamesManager.CreateGame();

            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count() == 0);
            
            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("")).success);
            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count() == 0);

            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("1")).success);
            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count() == 1
                && gamesManager.GetGameState(gameId).game!.Players.First()?.Name.Value == "1");

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("1")).success);

            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("2")).success);
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("3")).success);
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("4")).success);
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("5")).success);

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("6")).success);
        }

        [Fact]
        public void TestAddAndGetPlayersNonExistingGame()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            Assert.False(gamesManager.AddPlayerToGame(new GameId("mystery lobby"), new PlayerName("mr. lost")).success);
            Assert.False(gamesManager.GetGameState(new GameId("mystery lobby")).success);
        }
    }
}