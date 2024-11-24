using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.Models;
using SooshEgoServer.Services;
using Xunit;

namespace SooshEgoServer.Tests.Services
{
    // todo tests for game stage, trying to make moves in certain stage, etc.
    public class JoinAndLeaveGameTests
    {
        private readonly Mock<ILogger<GamesManager>> mockLogger;
        private readonly GamesManager gamesManager;

        public JoinAndLeaveGameTests()
        {
            mockLogger = new Mock<ILogger<GamesManager>>();
            gamesManager = new GamesManager(mockLogger.Object);
        }

        [Fact]
        public void CreateGame_ShouldGenerateUniqueIds()
        {
            PlayerName playerName = new("player1");

            (bool success1, GameId? gameId1, _) = gamesManager.CreateAndAddPlayerToGame(playerName);
            (bool success2, GameId? gameId2, _) = gamesManager.CreateAndAddPlayerToGame(playerName);

            Assert.True(success1);
            Assert.True(success2);
            Assert.NotNull(gameId1);
            Assert.NotNull(gameId2);
            Assert.NotEqual(gameId1, gameId2);
        }

        [Fact]
        public void CreateGame_ShouldFailWithEmptyPlayerName()
        {
            PlayerName emptyName = new("");

            (bool success, GameId? gameId, string error) = gamesManager.CreateAndAddPlayerToGame(emptyName);

            Assert.False(success);
            Assert.Null(gameId);
            Assert.False(string.IsNullOrWhiteSpace(error));
        }

        [Fact]
        public void AddPlayer_ShouldEnforcePlayerLimit()
        {
            PlayerName playerName = new("host");
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(playerName);

            Assert.NotNull(gameId);

            for (int i = 1; i < 5; i++)
            {
                string name = $"player{i}";
                Assert.True(gamesManager.AddPlayerToGame(gameId, new PlayerName(name)).success);
            }

            Assert.False(gamesManager.AddPlayerToGame(gameId, new PlayerName("player6")).success);
        }

        [Fact]
        public void AddPlayer_ShouldNotAllowDuplicateNames()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("host"));
            Assert.NotNull(gameId);

            bool firstAddSuccess = gamesManager.AddPlayerToGame(gameId, new("duplicate")).success;
            bool secondAddSuccess = gamesManager.AddPlayerToGame(gameId, new("duplicate")).success;

            Assert.True(firstAddSuccess);
            Assert.False(secondAddSuccess);
        }

        [Fact]
        public void Disconnect_ShouldCleanupEmptyGames()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.MarkPlayerConnected(gameId, new("player1"), "connection1");

            gamesManager.MarkPlayerDisconnectedAndCleanup("connection1");

            Assert.False(gamesManager.GetGameState(gameId).success);
        }

        [Fact]
        public void Disconnect_ShouldNotRemoveNonEmptyGames()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player2"));
            gamesManager.MarkPlayerConnected(gameId, new("player1"), "connection1");
            gamesManager.MarkPlayerConnected(gameId, new("player2"), "connection2");

            gamesManager.MarkPlayerDisconnectedAndCleanup("connection1");

            Assert.True(gamesManager.GetGameState(gameId).success);
        }

        [Fact]
        public void GameStateUpdated_ShouldFireCorrectly()
        {
            int eventFiredCount = 0;
            gamesManager.GameStateUpdated += (_, _) => eventFiredCount++;

            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("host"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player1"));
            gamesManager.MarkPlayerConnected(gameId, new("player1"), "connection1");

            Assert.Equal(3, eventFiredCount);
        }

        [Fact]
        public void StartGame_RequiresPlayers()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("host"));
            Assert.NotNull(gameId);

            (bool success, string _) = gamesManager.StartGame(gameId);
            Assert.False(success);

            gamesManager.AddPlayerToGame(gameId, new("player1"));
            (success, string _) = gamesManager.StartGame(gameId);
            Assert.True(success);
            Assert.True(gamesManager.GetGameState(gameId).game!.GameStage == GameStage.Round1);
        }

        // todo need a test for deleting rooms that have 1 person who's added but never connected
    }
}
