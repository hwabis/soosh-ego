using SooshEgoServer.Models;
using System.Text.Json;
using Xunit;

namespace SooshEgoServer.Tests.Models
{
    public class GameModelsTests
    {
        [Fact]
        public void Models_Serializable()
        {
            Game game = new(new("summoners rift"))
            {
                GameStage = GameStage.Round1
            };

            Player gragas = new(new("gragas"));
            gragas.CardsInHand.AddRange([new(CardType.MakiRoll1)]);
            gragas.CardsInPlay.AddRange([new(CardType.Tempura), new(CardType.Sashimi), new(CardType.Dumpling)]);

            Player jonx = new(new("jonx"))
            {
                ConnectionId = "jonx-connection-id"
            };

            game.Players.AddRange([gragas, jonx]);

            string json = JsonSerializer.Serialize(game);
            Game? deserializedGame = JsonSerializer.Deserialize<Game>(json);

            Assert.NotNull(deserializedGame);

            Assert.True(game.GameId.Value == "summoners rift");
            Assert.True(game.GameStage == GameStage.Round1);

            Assert.True(game.Players[0].Name.Value == "gragas");
            Assert.Null(game.Players[0].ConnectionId);
            Assert.True(game.Players[0].CardsInHand.Count == 1);
            Assert.True(game.Players[0].CardsInHand[0].CardType == CardType.MakiRoll1);
            Assert.True(game.Players[0].CardsInPlay.Count == 3);

            Assert.True(game.Players[1].Name.Value == "jonx");
            Assert.True(game.Players[1].ConnectionId == "jonx-connection-id");
            Assert.True(game.Players[1].CardsInHand.Count == 0);
            Assert.True(game.Players[1].CardsInPlay.Count == 0);
        }
    }
}
