using SooshEgoServer.Models;

namespace SooshEgoServer.Services
{
    public static class GamePointsCalculator
    {
        public static Dictionary<Player, int> CalculateRoundPoints(Game game, bool includePudding)
        {
            Dictionary<Player, int> playerPointsForThisRound = [];
            Dictionary<Player, int> makiRollCounts = [];
            Dictionary<Player, int> puddingCounts = [];

            foreach (Player player in game.Players)
            {
                playerPointsForThisRound.Add(player, 0);
                makiRollCounts.Add(player, 0);
                puddingCounts.Add(player, 0);

                int tempuraCount = 0;
                int sashimiCount = 0;
                int dumplingCount = 0;
                bool isWasabiActive = false;

                foreach (Card card in player.CardsInPlay)
                {
                    switch (card.CardType)
                    {
                        case CardType.Tempura:
                            tempuraCount++;
                            break;
                        case CardType.Sashimi:
                            sashimiCount++;
                            break;
                        case CardType.Dumpling:
                            dumplingCount++;
                            break;
                        case CardType.Wasabi:
                            isWasabiActive = true;
                            break;
                        case CardType.SalmonNigiri:
                            playerPointsForThisRound[player] += isWasabiActive ? 9 : 3;
                            isWasabiActive = false;
                            break;
                        case CardType.SquidNigiri:
                            playerPointsForThisRound[player] += isWasabiActive ? 6 : 2;
                            isWasabiActive = false;
                            break;
                        case CardType.EggNigiri:
                            playerPointsForThisRound[player] += isWasabiActive ? 3 : 1;
                            isWasabiActive = false;
                            break;
                        case CardType.MakiRoll3:
                            makiRollCounts[player] += 3;
                            break;
                        case CardType.MakiRoll2:
                            makiRollCounts[player] += 2;
                            break;
                        case CardType.MakiRoll1:
                            makiRollCounts[player] += 1;
                            break;
                        case CardType.Pudding:
                            puddingCounts[player]++;
                            break;
                        default:
                            break;
                    }
                }

                int tempuraSets = tempuraCount / 2;
                playerPointsForThisRound[player] += tempuraSets * 5;

                int sashimiSets = sashimiCount / 3;
                playerPointsForThisRound[player] += sashimiSets * 10;

                playerPointsForThisRound[player] += dumplingCount switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 3,
                    3 => 6,
                    4 => 10,
                    _ => 15,
                };
            }

            AddMakiRollPoints(makiRollCounts, playerPointsForThisRound);

            if (includePudding)
            {
                AddPuddingPoints(puddingCounts, playerPointsForThisRound);
            }

            return playerPointsForThisRound;
        }

        public static string GetGameWinnerName(Game game)
        {
            int maxPoints = game.Players.Select(player => player.PointsAtEndOfPreviousRound).Max();
            List<Player> playersWithMaxPoints = game.Players.Where(player => player.PointsAtEndOfPreviousRound == maxPoints).ToList();

            if (playersWithMaxPoints.Count == 1)
            {
                return playersWithMaxPoints[0].Name.Value;
            }

            int maxPudding = playersWithMaxPoints.Select(player => player.CardsInPlay.Count(
                card => card.CardType == CardType.Pudding)).Max();

            // Intentionally have no further tiebreaker than pudding count
            Player? winner = playersWithMaxPoints.FirstOrDefault(player =>
                player.CardsInPlay.Count(card => card.CardType == CardType.Pudding) == maxPudding);

            return winner?.Name.Value ?? throw new Exception("There was no winner name.");
        }

        private static void AddMakiRollPoints(
            Dictionary<Player, int> makiRollCounts, Dictionary<Player, int> playerPointsForThisRound)
        {
            int maxMaki = makiRollCounts.Values.Max();

            List<Player> firstPlaceMakiPlayers = [];

            if (maxMaki > 0)
            {
                firstPlaceMakiPlayers = makiRollCounts.Where(kvp => kvp.Value == maxMaki).Select(kvp => kvp.Key).ToList();
            }

            int secondMaxMaki = makiRollCounts.Values.Where(count => count < maxMaki).DefaultIfEmpty(0).Max();

            List<Player> secondPlaceMakiPlayers = [];

            if (secondMaxMaki > 0)
            {
                secondPlaceMakiPlayers = makiRollCounts.Where(kvp => kvp.Value == secondMaxMaki).Select(kvp => kvp.Key).ToList();
            }

            if (firstPlaceMakiPlayers.Count > 0)
            {
                int firstPlacePoints = 6 / firstPlaceMakiPlayers.Count;

                foreach (Player player in firstPlaceMakiPlayers)
                {
                    playerPointsForThisRound[player] += firstPlacePoints;
                }
            }

            if (secondPlaceMakiPlayers.Count > 0)
            {
                int secondPlacePoints = 3 / secondPlaceMakiPlayers.Count;

                foreach (Player player in secondPlaceMakiPlayers)
                {
                    playerPointsForThisRound[player] += secondPlacePoints;
                }
            }
        }

        private static void AddPuddingPoints(
            Dictionary<Player, int> puddingCounts, Dictionary<Player, int> playerPointsForThisRound)
        {
            int maxPudding = puddingCounts.Values.Max();
            int minPudding = puddingCounts.Values.Min();

            if (maxPudding == minPudding)
            {
                return;
            }

            List<Player> mostPuddingPlayers = puddingCounts.Where(kvp => kvp.Value == maxPudding).Select(kvp => kvp.Key).ToList();
            List<Player> leastPuddingPlayers = puddingCounts.Where(kvp => kvp.Value == minPudding).Select(kvp => kvp.Key).ToList();

            if (mostPuddingPlayers.Count > 0)
            {
                int pointsForMost = 6 / mostPuddingPlayers.Count;

                foreach (Player player in mostPuddingPlayers)
                {
                    playerPointsForThisRound[player] += pointsForMost;
                }
            }

            if (leastPuddingPlayers.Count > 0 && playerPointsForThisRound.Count > 2)
            {
                int pointsForLeast = -6 / leastPuddingPlayers.Count;

                foreach (Player player in leastPuddingPlayers)
                {
                    playerPointsForThisRound[player] += pointsForLeast;
                }
            }
        }
    }
}
