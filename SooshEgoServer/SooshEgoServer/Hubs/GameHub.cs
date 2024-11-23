﻿using Microsoft.AspNetCore.SignalR;
using SooshEgoServer.GameManagement;

namespace SooshEgoServer.Hubs
{
    // todo hub-level unit test?
    public class GameHub : Hub
    {
        private readonly IGamesManager gamesManager;

        public GameHub(IGamesManager gamesManager, GameUpdateNotifier gameUpdateNotifier)
        {
            this.gamesManager = gamesManager;
            this.gamesManager.GameStateUpdated += gameUpdateNotifier.NotifyGameStateUpdated;
        }

        public override async Task OnConnectedAsync()
        {
            string? gameId = Context.GetHttpContext()?.Request.Query["gameId"];
            string? playerName = Context.GetHttpContext()?.Request.Query["playerName"];

            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(playerName))
            {
                await SendError("Missing game ID or player name");
                return;
            }

            gamesManager.MarkPlayerConnected(new(gameId), new(playerName), Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gamesManager.MarkPlayerDisconnectedAndCleanup(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("Error", message);
        }
    }
}
