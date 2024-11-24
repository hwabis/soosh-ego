import { useSearchParams } from "react-router-dom";
import { connectToGame } from "../services/SignalRService";
import { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";
import { Game, GameStage } from "../models/Models";
import GameStatus from "./GameStatus";
import PlayerBox from "./PlayerBox";

const PlayScreen = () => {
  const [searchParams] = useSearchParams();
  const gameId = searchParams.get("gameId");
  const playerName = searchParams.get("playerName");

  const [game, setGame] = useState<Game>({
    gameId: { value: "" },
    gameStage: GameStage.Lobby,
    players: [],
  });
  const connectionRef = useRef<HubConnection | null>(null);
  const isConnectingRef = useRef(false);

  const [errorMessage, setErrorMessage] = useState<string>('');

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

      try {
        connectionRef.current = await connectToGame(
          gameId,
          playerName,
          updatedGame => setGame(updatedGame),
          error => setErrorMessage(error)
        );
      } catch (error) {
        setErrorMessage(String(error));
      }

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
    <div className="flex flex-col justify-center items-center h-screen bg-gray-100">
      <GameStatus
        game={game}
        isPlayerHost={game.players.length > 0 && playerName === game.players[0].name.value}
        errorMessage={errorMessage}
      />
      <div className="flex flex-wrap justify-center items-center gap-4">
        {game.players.map(player => (
          <PlayerBox
            key={player.name.value}
            player={player}
            isLocalPlayer={player.name.value === playerName}
          />
        ))}
      </div>
    </div>
  );
}

export default PlayScreen;
