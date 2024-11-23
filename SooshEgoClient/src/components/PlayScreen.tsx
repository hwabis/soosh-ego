import { useSearchParams } from "react-router-dom";
import { connectToGame, GameState } from "../services/SignalRService";
import { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";

const PlayScreen = () => {
  const [searchParams] = useSearchParams();
  const gameId = searchParams.get("gameId");
  const playerName = searchParams.get("playerName");

  const [gameState, setGameState] = useState<GameState>();
  const connectionRef = useRef<HubConnection | null>(null);
  const isConnectingRef = useRef(false);

  useEffect(() => {
    if (!gameId || !playerName) {
      return;
    }

    const cleanupConnection = async () => {
      if (connectionRef.current) {
        await connectionRef.current.stop();
        connectionRef.current = null;
      }
    };

    const connectAndSetConnection = async () => {
      if (isConnectingRef.current) {
        return;
      }

      isConnectingRef.current = true;

      connectionRef.current = await connectToGame(
        gameId,
        playerName,
        updatedGameState => setGameState(updatedGameState)
      );

      isConnectingRef.current = false;
    };

    cleanupConnection().then(() => {
      connectAndSetConnection();
    });

    return () => {
      cleanupConnection();
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
