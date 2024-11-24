import { Game } from "../models/Models";

interface GameStatusProps {
  game: Game;
  isPlayerHost: boolean;
  errorMessage: string;
  handleStartGame: () => void;
  isGameStarted: boolean;
}

const GameStatus = ({ game, isPlayerHost, errorMessage, handleStartGame, isGameStarted }: GameStatusProps) => {
  return (
    <div>
      <div className="absolute top-4 left-4">
        <div className="bg-red-900 rounded w-52 p-4">
          <label className="block font-medium text-white">Game ID: {game.gameId.value}</label>
          <label className="block font-medium text-white">Stage: {game.gameStage}</label>
          {isPlayerHost ? (
            <button
              className="w-full text-white rounded p-2 my-2 bg-green-600 hover:bg-green-700"
              onClick={handleStartGame}
              disabled={isGameStarted}>
              Start game!
            </button>
          ) : (
            <label className="block text-white my-2">Waiting for the host to start the game...</label>
          )}
        </div>
        <label className="block font-medium text-red-900 my-2">
          {errorMessage}
        </label>
      </div>
    </div >
  );
};

export default GameStatus;
