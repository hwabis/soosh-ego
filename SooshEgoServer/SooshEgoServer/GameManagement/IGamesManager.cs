using SooshEgoServer.GameManagement.Models;

namespace SooshEgoServer.GameManagement
{
    public interface IGamesManager
    {
        /// <summary>
        /// Only invoked on a successful operation.
        /// </summary>
        public event EventHandler<GameStateUpdatedEventArgs> GameStateUpdated;

        #region In-lobby

        public (bool success, GameId? gameId, string error) CreateAndAddPlayerToGame(PlayerName playerName);
        public (bool success, string errorMessage) AddPlayerToGame(GameId gameId, PlayerName playerName);

        public (bool success, Game? game) GetGameState(GameId gameId);

        /// <summary>
        /// Throws if gameId or playerName do not exist.
        /// </summary>
        public void MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId);
        /// <summary>
        /// Throws if playerConnectionId does not exist.
        /// </summary>
        public void MarkPlayerDisconnectedAndCleanup(string playerConnectionId);

        #endregion

        #region Gameplay

        #endregion
    }
}
