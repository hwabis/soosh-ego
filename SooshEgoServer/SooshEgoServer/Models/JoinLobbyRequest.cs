namespace SooshEgoServer.Models
{
    public class JoinLobbyRequest
    {
        public Guid LobbyId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
    }
}
