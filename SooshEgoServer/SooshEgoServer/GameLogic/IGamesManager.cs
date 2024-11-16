namespace SooshEgoServer.GameLogic
{
    public interface IGamesManager
    {
        public GameId? CreateGame();
        public bool JoinGame(GameId gameId, PlayerName playerName);
    }
}
