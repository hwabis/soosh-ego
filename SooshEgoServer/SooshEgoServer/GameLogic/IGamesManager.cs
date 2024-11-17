namespace SooshEgoServer.GameLogic
{
    public interface IGamesManager
    {
        public GameId CreateGame();

        public bool AddPlayerToGame(GameId gameId, PlayerName playerName);
        public IEnumerable<PlayerName>? GetPlayerNames(GameId gameId); // whether connected or disconnected

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId);
        public void MarkPlayerDisconnected(string playerConnectionId);
    }
}
