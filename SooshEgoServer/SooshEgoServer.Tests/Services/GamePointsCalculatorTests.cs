using Xunit;
using SooshEgoServer.Models;
using SooshEgoServer.Services;

namespace SooshEgoServer.Tests.Services
{
    public class GamePointsCalculatorTests
    {
        [Fact]
        public void TestPointsWithNoPlayerConditions()
        {
            Game game = new(new("henesys"));
            Player player1 = new(new("player1"));
            Player player2 = new(new("player2"));
            Player player3 = new(new("player3"));

            game.Players.Add(player1);
            game.Players.Add(player2);
            game.Players.Add(player3);

            player1.CardsInPlay =
            [
                new(CardType.Tempura),
                new(CardType.Tempura), // 5
                new(CardType.Tempura),
                new(CardType.Sashimi),
                new(CardType.Sashimi),
                new(CardType.Sashimi), // 15
                new(CardType.Sashimi),
                new(CardType.Dumpling), // 16
                new(CardType.Dumpling), // 18
                new(CardType.Dumpling), // 21
                new(CardType.Dumpling), // 25
                new(CardType.Dumpling), // 30
            ];

            player2.CardsInPlay =
            [
                new(CardType.Tempura),
                new(CardType.Sashimi),
                new(CardType.Sashimi),
                new(CardType.Dumpling), // 1
            ];

            player3.CardsInPlay =
            [
                new(CardType.SquidNigiri), // 3
                new(CardType.SalmonNigiri), // 5
                new(CardType.EggNigiri), // 6
                new(CardType.Wasabi),
                new(CardType.Wasabi),
                new(CardType.EggNigiri), // 9
                new(CardType.EggNigiri), // 10
                new(CardType.Wasabi),
                new(CardType.SquidNigiri), // 19
            ];

            Dictionary<Player, int> playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(30, playerPoints[player1]);
            Assert.Equal(1, playerPoints[player2]);
            Assert.Equal(19, playerPoints[player3]);
        }

        [Fact]
        public void TestMakiRollPoints()
        {
            Game game = new(new("henesys"));
            Player player1 = new(new("player1"));
            Player player2 = new(new("player2"));
            Player player3 = new(new("player3"));

            game.Players.Add(player1);
            game.Players.Add(player2);
            game.Players.Add(player3);

            player1.CardsInPlay.Add(new(CardType.MakiRoll3));
            player2.CardsInPlay.Add(new(CardType.MakiRoll2));
            player3.CardsInPlay.AddRange([new(CardType.MakiRoll1), new(CardType.MakiRoll1)]);

            Dictionary<Player, int> playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(1, playerPoints[player2]);
            Assert.Equal(1, playerPoints[player3]);

            player2.CardsInPlay =
            [
                new(CardType.MakiRoll3),
            ];

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(3, playerPoints[player1]);
            Assert.Equal(3, playerPoints[player2]);
            Assert.Equal(3, playerPoints[player3]);

            player2.CardsInPlay.Clear();
            player3.CardsInPlay.Clear();

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);
            Assert.Equal(0, playerPoints[player3]);

            player2.CardsInPlay.Add(new(CardType.MakiRoll1));

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(3, playerPoints[player2]);
            Assert.Equal(0, playerPoints[player3]);

            player1.CardsInPlay.Clear();
            player2.CardsInPlay.Clear();

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(0, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);
            Assert.Equal(0, playerPoints[player3]);
        }

        [Fact]
        public void TestAddPuddingPoints()
        {
            Game game = new(new("henesys"));
            Player player1 = new(new("player1"));
            Player player2 = new(new("player2"));

            game.Players.Add(player1);
            game.Players.Add(player2);

            Dictionary<Player, int> playerPoints = GamePointsCalculator.CalculateRoundPoints(game, false);

            Assert.Equal(0, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);

            player1.CardsInPlay.Add(new(CardType.Pudding));

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, true);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);

            Player player3 = new(new("player3"));
            game.Players.Add(player3);

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, true);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(-3, playerPoints[player2]);
            Assert.Equal(-3, playerPoints[player3]);

            player2.CardsInPlay.Add(new(CardType.Pudding));

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, true);

            Assert.Equal(3, playerPoints[player1]);
            Assert.Equal(3, playerPoints[player2]);
            Assert.Equal(-6, playerPoints[player3]);

            player1.CardsInPlay.Add(new(CardType.Pudding));

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, true);

            Assert.Equal(6, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);
            Assert.Equal(-6, playerPoints[player3]);

            player1.CardsInPlay.Clear();
            player2.CardsInPlay.Clear();

            playerPoints = GamePointsCalculator.CalculateRoundPoints(game, true);

            Assert.Equal(0, playerPoints[player1]);
            Assert.Equal(0, playerPoints[player2]);
            Assert.Equal(0, playerPoints[player3]);
        }

        [Fact]
        public void TestGetGameWinnerName()
        {
            Game game = new(new("henesys"));
            Player player1 = new(new("player1"));
            Player player2 = new(new("player2"));

            game.Players.Add(player1);
            game.Players.Add(player2);

            player1.PointsAtEndOfPreviousRound = 1;
            player2.PointsAtEndOfPreviousRound = 0;

            string winner1 = GamePointsCalculator.GetGameWinnerName(game);
            Assert.True(winner1 == "player1");

            player1.PointsAtEndOfPreviousRound = 2;
            player2.PointsAtEndOfPreviousRound = 2;
            player2.CardsInPlay.Add(new(CardType.Pudding));

            string winner2 = GamePointsCalculator.GetGameWinnerName(game);
            Assert.True(winner2 == "player2");
        }

    }
}
