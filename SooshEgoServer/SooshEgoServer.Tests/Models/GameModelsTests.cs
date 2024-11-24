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
            Game game = new(new("bomba"))
            {
                Players =
                [
                    new(new("gragas")),
                    new(new("jonx"))
                    {
                        ConnectionId = "jonx-connection-id"
                    },
                ]
            };

            string json = JsonSerializer.Serialize(game);
            Game? deserializedGame = JsonSerializer.Deserialize<Game>(json);

            Assert.NotNull(deserializedGame);

            Assert.True(game.GameId.Value == "bomba");

            Assert.True(game.Players[0].Name.Value == "gragas");
            Assert.Null(game.Players[0].ConnectionId);

            Assert.True(game.Players[1].Name.Value == "jonx");
            Assert.True(game.Players[1].ConnectionId == "jonx-connection-id");

            // todo as we add more state fields to each player, make sure they are serialized correctly
        }
    }
}
