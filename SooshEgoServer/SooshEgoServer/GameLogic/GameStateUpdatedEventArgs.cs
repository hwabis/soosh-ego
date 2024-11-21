using SooshEgoServer.GameLogic.Models;

namespace SooshEgoServer.GameLogic
{
    public class GameStateUpdatedEventArgs(Game game) : EventArgs
    {
        public Game Game { get; } = game;
    }
}