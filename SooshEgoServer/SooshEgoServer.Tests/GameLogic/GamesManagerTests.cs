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
        public void TestGameCreation()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            (bool success1, GameId? gameId1, _) = gamesManager.CreateAndAddPlayerToGame(new("fangblade"));
            (bool success2, GameId? gameId2, _) = gamesManager.CreateAndAddPlayerToGame(new("fangblade"));

            Assert.True(success1);
            Assert.True(success2);
            Assert.NotNull(gameId1);
            Assert.NotNull(gameId2);

            Assert.NotEqual(gameId1.Value, gameId2.Value);

            (bool success3, GameId? gameId3, string error3) = gamesManager.CreateAndAddPlayerToGame(new(""));

            Assert.False(success3);
            Assert.Null(gameId3);
            Assert.False(string.IsNullOrEmpty(error3));
        }

        [Fact]
        public void TestAddAndGetPlayers()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            (bool _, GameId? gameId, string _) = gamesManager.CreateAndAddPlayerToGame(new("asdf"));

            Assert.NotNull(gameId);

            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count == 1
                && gamesManager.GetGameState(gameId).game!.Players[0].Name.Value == "asdf");

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("")).success);
            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count == 1);

            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy2")).success);
            Assert.True(gamesManager.GetGameState(gameId).success
                && gamesManager.GetGameState(gameId).game!.Players.Count == 2
                && gamesManager.GetGameState(gameId).game!.Players[1].Name.Value == "guy2");

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy2")).success);

            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy3")).success);
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy4")).success);
            Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy5")).success);
            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("guy6")).success);

            Assert.True(gamesManager.GetGameState(gameId).game!.Players.Count == 5);
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

            (bool success1, GameId? gameId1, _) = gamesManager.CreateAndAddPlayerToGame(new("zhou"));
            Assert.True(gameStateUpdateCount == 1);

            Assert.NotNull(gameId1);

            Assert.Null(gamesManager.GetGameState(gameId1).game!.Players[0].ConnectionId);
            gamesManager.MarkPlayerConnected(gameId1, new("zhou"), "zhou-connection");
            Assert.True(gameStateUpdateCount == 2);

            gamesManager.AddPlayerToGame(gameId1, new("ayaya"));
            Assert.True(gameStateUpdateCount == 3);

            gamesManager.MarkPlayerConnected(gameId1, new("ayaya"), "ayaya-connection");
            Assert.True(gameStateUpdateCount == 4);
            Assert.True(gamesManager.GetGameState(gameId1).game!.Players[1].ConnectionId == "ayaya-connection");

            gamesManager.AddPlayerToGame(gameId1, new("bumba"));
            Assert.True(gameStateUpdateCount == 5);

            gamesManager.MarkPlayerConnected(gameId1, new("bumba"), "bumba-connection");
            Assert.True(gameStateUpdateCount == 6);
            Assert.True(gamesManager.GetGameState(gameId1).game!.Players[2].ConnectionId == "bumba-connection");

            gamesManager.MarkPlayerDisconnectedAndCleanup("ayaya-connection");
            Assert.True(gameStateUpdateCount == 7);
            Assert.Null(gamesManager.GetGameState(gameId1).game!.Players[1].ConnectionId);
            Assert.True(gamesManager.GetGameState(gameId1).game!.Players[0].ConnectionId == "zhou-connection");
            Assert.True(gamesManager.GetGameState(gameId1).game!.Players[2].ConnectionId == "bumba-connection");

            (bool success2, GameId? gameId2, _) = gamesManager.CreateAndAddPlayerToGame(new("stanley"));

            Assert.NotNull(gameId2);

            Assert.True(gameStateUpdateCount == 8);

            gamesManager.AddPlayerToGame(gameId2, new("ayaya"));
            Assert.True(gameStateUpdateCount == 9);
            Assert.Null(gamesManager.GetGameState(gameId2).game!.Players[0].ConnectionId);
            Assert.Null(gamesManager.GetGameState(gameId2).game!.Players[1].ConnectionId);

            gamesManager.AddPlayerToGame(gameId2, new("bumba"));
            Assert.True(gameStateUpdateCount == 10);

            gamesManager.MarkPlayerConnected(gameId2, new("bumba"), "bumba-connection");
            Assert.True(gameStateUpdateCount == 11);
            Assert.True(gamesManager.GetGameState(gameId2).game!.Players[2].ConnectionId == "bumba-connection");

            gamesManager.MarkPlayerConnected(gameId2, new("ayaya"), "ayaya-connection");
            Assert.True(gameStateUpdateCount == 12);
            Assert.True(gamesManager.GetGameState(gameId2).game!.Players[1].ConnectionId == "ayaya-connection");

            gamesManager.MarkPlayerDisconnectedAndCleanup("nobody-has-this-connection");
            Assert.True(gameStateUpdateCount == 12);
        }

        [Fact]
        public void TestGameDisconnectCleanup()
        {
            Mock<ILogger<GamesManager>> mockLogger = new();
            GamesManager gamesManager = new(mockLogger.Object);

            (bool _, GameId? gameId1, _) = gamesManager.CreateAndAddPlayerToGame(new("bombito"));

            Assert.NotNull(gameId1);

            gamesManager.AddPlayerToGame(gameId1, new("ayaya"));
            gamesManager.MarkPlayerConnected(gameId1, new("ayaya"), "ayaya-connection");
            gamesManager.MarkPlayerDisconnectedAndCleanup("ayaya-connection");
            Assert.False(gamesManager.GetGameState(gameId1).success);

            (bool _, GameId? gameId2, _) = gamesManager.CreateAndAddPlayerToGame(new("zod"));

            Assert.NotNull(gameId2);

            gamesManager.MarkPlayerConnected(gameId2, new("zod"), "zod-connection");

            gamesManager.AddPlayerToGame(gameId2, new("bumba"));
            gamesManager.MarkPlayerConnected(gameId2, new("bumba"), "bumba-connection");

            gamesManager.MarkPlayerDisconnectedAndCleanup("bumba-connection");
            Assert.True(gamesManager.GetGameState(gameId2).success);
            Assert.NotNull(gamesManager.GetGameState(gameId2).game);
            Assert.True(gamesManager.GetGameState(gameId2).game!.Players.Count == 2
                && gamesManager.GetGameState(gameId2).game!.Players[0].Name.Value == "zod");

            gamesManager.MarkPlayerDisconnectedAndCleanup("zod-connection");
            Assert.False(gamesManager.GetGameState(gameId2).success);
        }
    }
}