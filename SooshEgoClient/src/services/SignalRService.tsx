import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { Game } from "../models/Models";

export const connectToGame = async (
   gameId: string,
   playerName: string,
   onGameUpdated: (updatedGame: Game) => void,
   onError: (error: string) => void): Promise<HubConnection> => {
   const connection = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/game-hub?gameId=${gameId}&playerName=${playerName}`)
      .build();

   connection.on("GameStateUpdated", (updatedGame: Game) => {
      console.log("SignalR game state updated");
      onGameUpdated(updatedGame);
   });

   connection.on("Error", (error: string) => {
      console.error("SignalR error: ", error);
      onError(error);
   });

   connection.onclose(error => {
      if (error) {
         console.error("SignalR disconnected with error: ", error);
         return;
      }

      console.log("SignalR disconnected");
   });

   console.log("SignalR connecting");
   await connection.start();
   console.log("SignalR connected");

   return connection;
};
