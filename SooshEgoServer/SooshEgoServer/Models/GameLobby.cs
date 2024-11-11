namespace SooshEgoServer.Models
{
    public class GameLobby
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public List<Player> Players { get; private set; } = [];
    }
}
