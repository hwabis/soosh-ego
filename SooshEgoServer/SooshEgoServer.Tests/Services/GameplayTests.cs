﻿using Microsoft.Extensions.Logging;
using Moq;
using SooshEgoServer.Models;
using SooshEgoServer.Services;
using Xunit;

namespace SooshEgoServer.Tests.Services
{
    public class GameplayTests
    {
        private readonly Mock<ILogger<GamesManager>> mockLogger;
        private readonly GamesManager gamesManager;

        public GameplayTests()
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

            (bool success1, string _) = gamesManager.PlayCard(gameId, new("player1"), 4, null);
            Assert.True(success1);

            List<Card> oldPlayer1Cards = [.. gamesManager.GetGameState(gameId).game!.Players[0].CardsInHand];

            (bool success2, string _) = gamesManager.PlayCard(gameId, new("player2"), 5, null);
            Assert.True(success2);

            (bool success3, string _) = gamesManager.PlayCard(gameId, new("player3"), 0, null);
            Assert.True(success3);

            // At this point, player 1's hand becomes player 2's
            List<Card> player2Hand = gamesManager.GetGameState(gameId).game!.Players[1].CardsInHand;

            for (int i = 0; i < player2Hand.Count; i++)
            {
                Assert.True(oldPlayer1Cards[i].CardType == player2Hand[i].CardType);
            }
        }

        // todo chopsticks can play 2

        // todo scoring

        [Fact]
        public void StartGame_NewRound_NewGame()
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
