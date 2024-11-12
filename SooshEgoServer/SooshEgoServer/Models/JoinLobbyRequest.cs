namespace SooshEgoServer.Models
{
    // todo also remove this
    public class JoinLobbyRequest
    {
        public Guid LobbyId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
    }
}
