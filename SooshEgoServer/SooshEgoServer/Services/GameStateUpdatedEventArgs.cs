using SooshEgoServer.Models;

namespace SooshEgoServer.Services
{
    public class GameStateUpdatedEventArgs(Game game) : EventArgs
    {
        public Game Game { get; } = game;
    }
}