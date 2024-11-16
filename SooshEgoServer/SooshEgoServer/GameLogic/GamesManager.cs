namespace SooshEgoServer.GameLogic
{
    public class GamesManager : IGamesManager
    {
        public GameId? CreateGame()
        {
            return new("lol");
        }

        public bool JoinGame(GameId gameId, PlayerName playerName)
        {
            return true;
        }
    }
}
