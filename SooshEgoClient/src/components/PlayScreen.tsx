import { useSearchParams } from "react-router-dom";
import { connectToGame, GameState } from "../services/WebSocketService";
import { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";

const PlayScreen = () => {
  const [searchParams] = useSearchParams();
  const gameId = searchParams.get("gameId");
  const playerName = searchParams.get("playerName");

  const [gameState, setGameState] = useState<GameState>();
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    if (!gameId || !playerName) {
      return;
    }

    const connectAndSetConnection = async () => {
      const connection = await connectToGame(
        gameId,
        playerName,
        updatedGameState => setGameState(updatedGameState),
        error => console.error(error)
      );

      connectionRef.current = connection;
    };

    connectAndSetConnection();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }
    };
  }, [gameId, playerName]);

  if (!gameId || !playerName) {
    return (<p>Game ID or player name is missing from URL!</p>);
  }

  return (
    <div>
      <h1>Play Screen</h1>
      <p>Game ID: {gameId}</p>
      <p>Player Name: {playerName}</p>
    </div>
  );
}

export default PlayScreen;
