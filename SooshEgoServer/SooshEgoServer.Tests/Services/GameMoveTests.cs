using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.Models;
using SooshEgoServer.Services;
using Xunit;

namespace SooshEgoServer.Tests.Services
{
    public class GameMoveTests
    {
        private readonly Mock<ILogger<GamesManager>> mockLogger;
        private readonly GamesManager gamesManager;

        public GameMoveTests()
        {
            mockLogger = new Mock<ILogger<GamesManager>>();
            gamesManager = new GamesManager(mockLogger.Object);
        }

        [Fact]
        public void PlayCard_ShouldNotAllowTwoMoves()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player2"));
            gamesManager.StartGame(gameId);

            (bool success1, string _) = gamesManager.PlayCard(gameId, new("player1"), 4, null);
            Assert.True(success1);

            (bool success2, string _) = gamesManager.PlayCard(gameId, new("player1"), 4, null);
            Assert.False(success2);
        }

        [Fact]
        public void PlayCard_ShouldRotateHandsToTheRight()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player2"));
            gamesManager.AddPlayerToGame(gameId, new("player3"));
            gamesManager.StartGame(gameId);

            Card player1CardToPlay = gamesManager.GetGameState(gameId).game!.Players[0].CardsInHand[4];

            (bool success1, string _) = gamesManager.PlayCard(gameId, new("player1"), 4, null);
            Assert.True(success1);

            List<Card> player1HandAfterPlay = [.. gamesManager.GetGameState(gameId).game!.Players[0].CardsInHand];
            player1HandAfterPlay.Remove(player1CardToPlay);

            (bool success2, string _) = gamesManager.PlayCard(gameId, new("player2"), 5, null);
            Assert.True(success2);

            (bool success3, string _) = gamesManager.PlayCard(gameId, new("player3"), 0, null);
            Assert.True(success3);

            // At this point, player 1's hand becomes player 2's
            List<Card> player2NewHand = gamesManager.GetGameState(gameId).game!.Players[1].CardsInHand;

            for (int i = 0; i < player2NewHand.Count; i++)
            {
                Assert.True(player1HandAfterPlay[i].CardType == player2NewHand[i].CardType);
            }
        }

        [Fact]
        public void PlayCard_ShoudlAllow2WithChopsticks()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player2"));
            gamesManager.StartGame(gameId);

            gamesManager.GetGameState(gameId).game!.Players[0].CardsInPlay.Add(new(CardType.Chopsticks));

            (bool success1, string _) = gamesManager.PlayCard(gameId, new("player1"), 8, 9);
            Assert.True(success1);
        }

        [Fact]
        public void StartGame_ToRestartGame()
        {
            (bool _, GameId? gameId, _) = gamesManager.CreateAndAddPlayerToGame(new("player1"));
            Assert.NotNull(gameId);

            gamesManager.AddPlayerToGame(gameId, new("player2"));

            for (int i = 0; i < 3; i++)
            {
                (bool success, string _) = gamesManager.StartGame(gameId);
                Assert.True(success);

                int startingHandSize = gamesManager.GetGameState(gameId).game!.Players[0].CardsInHand.Count;

                for (int j = 0; j < startingHandSize; j++)
                {
                    foreach (Player player in gamesManager.GetGameState(gameId).game!.Players)
                    {
                        (bool success1, string _) = gamesManager.PlayCard(gameId, player.Name, 0, null);
                        Assert.True(success1);
                    }
                }

                Assert.True(gamesManager.GetGameState(gameId).game!.NumberOfRoundsCompleted == i + 1);
            }

            (bool success2, string _) = gamesManager.StartGame(gameId);
            Assert.True(success2);
            Assert.True(gamesManager.GetGameState(gameId).game!.NumberOfRoundsCompleted == 0);
        }
    }
}
