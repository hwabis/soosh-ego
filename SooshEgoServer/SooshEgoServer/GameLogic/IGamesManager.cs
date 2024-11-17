using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.GameLogic
{
    public interface IGamesManager
    {
        public event EventHandler<GameStateUpdatedEventArgs> GameStateUpdated;

        public GameId CreateGame();

        public (bool success, string errorMessage) AddPlayerToGame(GameId gameId, PlayerName playerName);
        public (bool success, Game? game) GetGameState(GameId gameId);

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId);
        public void MarkPlayerDisconnected(string playerConnectionId);
    }
}
