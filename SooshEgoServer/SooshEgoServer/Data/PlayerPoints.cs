namespace SooshEgoServer.Data
{
    public class PlayerPoints
    {
        public int Id { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int Points { get; set; }

        public int CompletedGameId { get; set; }
        public CompletedGame CompletedGame { get; set; } = null!;
    }
}
