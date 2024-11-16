﻿namespace SooshEgoServer.GameLogic
{
    public class Player
    {
        public readonly PlayerName Name;

        public Player(PlayerName name)
        {
            Name = name;
        }
    }

    public record PlayerName(string Value);
}