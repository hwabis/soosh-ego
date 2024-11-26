import { useState } from "react";
import { Game, GameStage } from "../models/Models";

interface GameStatusProps {
  game: Game;
  isPlayerHost: boolean;
  errorMessage: string;
  handleStartGame: () => void;
}

const GameStatus = ({ game, isPlayerHost, errorMessage, handleStartGame }: GameStatusProps) => {
  const [copyButtonText, setCopyButtonText] = useState<string>("Copy game ID");

  const isGameStartable = game.gameStage !== GameStage.Playing;
  const stageDescription = game.gameStage === GameStage.Playing
    ? `Round ${game.numberOfRoundsCompleted + 1} / 3`
    : game.gameStage;

  const copyGameIdToClipboard = () => {
    navigator.clipboard.writeText(game.gameId.value);
    setCopyButtonText("Copied!");
    setTimeout(() => {
      setCopyButtonText("Copy game ID");
    }, 2000);
  };

  return (
    <div>
      <div className="absolute top-4 left-4">
        <div className="bg-red-900 rounded w-52 p-4">
          <p className="block font-medium text-white">Game ID: {game.gameId.value}</p>
          {
            isGameStartable && (
              <button
                className="block w-full text-white rounded p-2 my-2 bg-orange-600 hover:bg-orange-700"
                onClick={copyGameIdToClipboard}
              >
                {copyButtonText}
              </button>)
          }
          <p className="block font-medium text-white">Stage: {stageDescription}</p>
          {isPlayerHost && isGameStartable && (
            <button
              className="w-full text-white rounded p-2 my-2 bg-green-600 hover:bg-green-700"
              onClick={handleStartGame}
            >
              Start game!
            </button>
          )}
          {!isPlayerHost && isGameStartable && (
            <p className="block text-white my-2">Waiting for the host to start the game...</p>
          )}
        </div>
        <p className="block font-medium text-red-900 my-2">
          {errorMessage}
        </p>
      </div>
    </div >
  );
};

export default GameStatus;
