import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

export interface GameState {
   value: string; // todo
}

export const connectToGame = async (
   gameId: string,
   playerName: string,
   onGameStateUpdated: (gameState: GameState) => void,
   onError: (error: string) => void): Promise<HubConnection> => {
   const connection = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/game-hub?gameId=${gameId}&playerName=${playerName}`)
      .build();

   connection.on("GameStateUpdated", (gameState: GameState) => {
      onGameStateUpdated(gameState);
   });

   connection.on("Error", (error: string) => {
      onError(error);
   });

   connection.onclose(error => {
      console.log("SignalR disconnected: ", error);
   });

   await connection.start()
      .then(() => console.log("SignalR connection established"))
      .catch((error) => console.error("SignalR connection error:", error));

   return connection;
};
