using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.GameLogic
{
    public class GameStateUpdatedEventArgs : EventArgs
    {
        public Game Game { get; }

        public GameStateUpdatedEventArgs(Game game)
        {
            Game = game;
        }
    }
}