using SooshEgoServer.GameManagement.Models;

namespace SooshEgoServer.GameManagement
{
    public class GameStateUpdatedEventArgs(Game game) : EventArgs
    {
        public Game Game { get; } = game;
    }
}