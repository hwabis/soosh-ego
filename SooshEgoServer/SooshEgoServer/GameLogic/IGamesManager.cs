using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.GameLogic
{
    public interface IGamesManager
    {
        public event EventHandler<GameStateUpdatedEventArgs> GameStateUpdated;

        #region In-lobby

        public GameId CreateGame();
        public void DeleteGame(GameId gameId);

        public (bool success, string errorMessage) AddPlayerToGame(GameId gameId, PlayerName playerName);
        public (bool success, Game? game) GetGameState(GameId gameId);

        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId);
        public void MarkPlayerDisconnectedAndCleanup(string playerConnectionId);

        #endregion

        #region Gameplay

        #endregion
    }
}
