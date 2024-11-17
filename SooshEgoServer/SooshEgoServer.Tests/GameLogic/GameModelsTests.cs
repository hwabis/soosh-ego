using SooshEgoServer.GameLogic.Models;
using System.Text.Json;
using Xunit;

namespace SooshEgoServer.Tests.GameLogic
{
    public class GameModelsTests
    {
        [Fact]
        public void TestModelsSerializable()
        {
            Game game = new(new("bomba"))
            {
                Players = new List<Player>
                {
                    new Player(new("gragas")),
                    new Player(new("jonx")),
                }
            };

            string json = JsonSerializer.Serialize(game);
            Game? deserializedGame = JsonSerializer.Deserialize<Game>(json);

            Assert.NotNull(deserializedGame);

            Assert.True(game.GameId.Value == "bomba");
            Assert.True(game.Players[0].Name.Value == "gragas");
            Assert.True(game.Players[1].Name.Value == "jonx");

            // todo as we add more state fields to each player, make sure they are serialized correctly
        }
    }
}
