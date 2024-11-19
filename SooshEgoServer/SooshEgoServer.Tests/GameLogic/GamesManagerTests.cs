using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.GameLogic;
using SooshEgoServer.GameLogic.Models;
using Xunit;

namespace SooshEgoServer.Tests.GameLogic
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

        [Fact]
        public void TestGameStateEventAndConnections()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            int gameStateUpdateCount = 0;
            gamesManager.GameStateUpdated += (_, _) => gameStateUpdateCount++;

            GameId gameId = gamesManager.CreateGame();
            Assert.True(gameStateUpdateCount == 0);

            gamesManager.AddPlayerToGame(gameId, new("ayaya"));
            Assert.True(gameStateUpdateCount == 1);
            Assert.Null(gamesManager.GetGameState(gameId).game!.Players[0].ConnectionId);

            gamesManager.MarkPlayerConnected(gameId, new("ayaya"), "ayaya-connection");
            Assert.True(gameStateUpdateCount == 2);
            Assert.True(gamesManager.GetGameState(gameId).game!.Players[0].ConnectionId == "ayaya-connection");

            gamesManager.AddPlayerToGame(gameId, new("bumba"));
            Assert.True(gameStateUpdateCount == 3);

            gamesManager.MarkPlayerConnected(gameId, new("bumba"), "bumba-connection");
            Assert.True(gameStateUpdateCount == 4);
            Assert.True(gamesManager.GetGameState(gameId).game!.Players[1].ConnectionId == "bumba-connection");

            gamesManager.MarkPlayerDisconnectedAndCleanup("ayaya-connection");
            Assert.True(gameStateUpdateCount == 5);
            Assert.Null(gamesManager.GetGameState(gameId).game!.Players[0].ConnectionId);
            Assert.True(gamesManager.GetGameState(gameId).game!.Players[1].ConnectionId == "bumba-connection");
        }

        [Fact]
        public void TestGameDisconnectCleanup()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            int gameStateUpdateCount = 0;
            gamesManager.GameStateUpdated += (_, _) => gameStateUpdateCount++;

            GameId gameId1 = gamesManager.CreateGame();

            gamesManager.AddPlayerToGame(gameId1, new("ayaya"));
            gamesManager.MarkPlayerConnected(gameId1, new("ayaya"), "ayaya-connection");
            gamesManager.MarkPlayerDisconnectedAndCleanup("ayaya-connection");
            Assert.False(gamesManager.GetGameState(gameId1).success);

            GameId gameId2 = gamesManager.CreateGame();

            gamesManager.AddPlayerToGame(gameId2, new("ayaya"));
            gamesManager.MarkPlayerConnected(gameId2, new("ayaya"), "ayaya-connection");
            gamesManager.AddPlayerToGame(gameId2, new("bumba"));
            gamesManager.MarkPlayerConnected(gameId2, new("bumba"), "bumba-connection");

            gamesManager.MarkPlayerDisconnectedAndCleanup("bumba-connection");
            Assert.True(gamesManager.GetGameState(gameId2).success);

            gamesManager.MarkPlayerDisconnectedAndCleanup("ayaya-connection");
            Assert.False(gamesManager.GetGameState(gameId2).success);
        }
    }
}