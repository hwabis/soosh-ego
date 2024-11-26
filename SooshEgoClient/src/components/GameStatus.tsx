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
  const isGameJoinable = isGameStartable && game.numberOfRoundsCompleted === 0;
  const stageDescription = (() => {
    switch (game.gameStage) {
      case GameStage.Playing:
        return `Round ${game.numberOfRoundsCompleted + 1} / 3`;
      case GameStage.Waiting:
        return `Waiting for round ${game.numberOfRoundsCompleted + 1}`;
      default:
        return game.gameStage;
    }
  })();
  const startButtonText = (() => {
    switch (game.gameStage) {
      case GameStage.Waiting:
        return "Start the next round!";
      case GameStage.Finished:
        return "Start a new game!";
      default:
        return "";
    }
  })();

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
            isGameJoinable && (
              <button
                className="block w-full text-white rounded p-2 my-2 bg-orange-600 hover:bg-orange-700"
                onClick={copyGameIdToClipboard}
              >
                {copyButtonText}
              </button>)
          }
          <p className="block font-medium text-white">{stageDescription}</p>
          {isPlayerHost && isGameStartable && (
            <button
              className="w-full text-white rounded p-2 my-2 bg-green-600 hover:bg-green-700"
              onClick={handleStartGame}
            >
              {startButtonText}
            </button>
          )}
          {!isPlayerHost && game.gameStage === GameStage.Waiting && (
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
