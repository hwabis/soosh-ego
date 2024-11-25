using SooshEgoServer.Models;

namespace SooshEgoServer.Services
{
    public interface IGamesManager
    {
        /// <summary>
        /// Only invoked on a successful operation.
        /// </summary>
        public event EventHandler<GameStateUpdatedEventArgs> GameStateUpdated;

        #region General

        public (bool success, Game? game) GetGameState(GameId gameId);
        public (bool success, string error) MarkPlayerConnected(GameId gameId, PlayerName playerName, string connectionId);
        public (bool success, string error) MarkPlayerDisconnectedAndCleanup(string playerConnectionId);

        #endregion

        #region Lobby

        public (bool success, GameId? gameId, string error) CreateAndAddPlayerToGame(PlayerName playerName);
        public (bool success, string error) AddPlayerToGame(GameId gameId, PlayerName playerName);
        public (bool success, string error) StartGame(GameId gameId);

        #endregion

        #region Gameplay

        public (bool success, string error) PlayCard(
            GameId gameId, PlayerName playername, int indexOfCardInHand, int? indexOfSecondCardInHandUsingChopsticks);

        #endregion
    }
}
