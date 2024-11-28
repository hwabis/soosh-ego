import { useSearchParams } from "react-router-dom";
import { connectToGame } from "../services/SignalRService";
import { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";
import { Game, GameStage } from "../models/Models";
import GameStatus from "./GameStatus";
import PlayerBox from "./PlayerBox";
import { playCard, startGame } from "../services/ApiService";
import CardSelection from "./CardSelection";

const PlayScreen = () => {
  const [searchParams] = useSearchParams();
  const gameId = searchParams.get("gameId");
  const playerName = searchParams.get("playerName");

  const [game, setGame] = useState<Game>({
    gameId: { value: "" },
    gameStage: GameStage.Waiting,
    numberOfRoundsCompleted: 0,
    players: [],
    winnerName: null,
  });
  const connectionRef = useRef<HubConnection | null>(null);
  const isConnectingRef = useRef(false);

  const [errorMessage, setErrorMessage] = useState<string>('');

  const localPlayer = game.players.find((p) => p.name.value === playerName);

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

    void cleanupConnection().then(async () => {
      await connectAndSetConnection();
    });

    return () => {
      void cleanupConnection();
    };
  }, [gameId, playerName]);

  if (!gameId || !playerName) {
    return (<p>Game ID or player name is missing from URL!</p>);
  }

  const handleStartGame = async () => {
    const success = await startGame(gameId, error => setErrorMessage(error));
    if (success) {
      setErrorMessage("");
    }
  }

  const handlePlayCard = async (selectedIndices: number[]) => {
    if (selectedIndices.length === 0) {
      return;
    }

    const [card1Index, card2Index] = selectedIndices;

    const success = await playCard(gameId, playerName, card1Index, card2Index ?? null,
      (error: string) => setErrorMessage(error));
    if (success) {
      setErrorMessage("");
    }
  };

  return (
    <div className="flex flex-col justify-center items-center h-screen bg-gray-100">
      <GameStatus
        game={game}
        isPlayerHost={game.players.length > 0 && playerName === game.players[0].name.value}
        errorMessage={errorMessage}
        handleStartGame={() => void handleStartGame()}
      />
      {game.winnerName && (
        <div className="font-bold text-lg my-8">{`The winner is ${game.winnerName}!`}</div>
      )}
      <div className="flex flex-wrap justify-center items-center gap-4">
        {game.players.map(player => (
          <PlayerBox
            key={player.name.value}
            player={player}
            isLocalPlayer={player.name.value === playerName}
            gameStage={game.gameStage}
          />
        ))}
      </div>
      {localPlayer && (
        <div className="absolute bottom-4 left-0 right-0">
          <CardSelection
            cards={localPlayer.cardsInHand}
            selectionLimit={1} // todo chopsticks
            onConfirm={selectedIndices => void handlePlayCard(selectedIndices)}
          />
        </div>
      )}
    </div>
  );
}

export default PlayScreen;
