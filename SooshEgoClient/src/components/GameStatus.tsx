import { Game, GameStage } from "../models/Models";

interface GameStatusProps {
  game: Game;
  isPlayerHost: boolean;
  errorMessage: string;
  handleStartGame: () => void;
}

const GameStatus = ({ game, isPlayerHost, errorMessage, handleStartGame }: GameStatusProps) => {
  const isInLobby = game.gameStage === GameStage.Lobby;

  return (
    <div>
      <div className="absolute top-4 left-4">
        <div className="bg-red-900 rounded w-52 p-4">
          <p className="block font-medium text-white">Game ID: {game.gameId.value}</p>
          <p className="block font-medium text-white">Stage: {game.gameStage}</p>
          {isPlayerHost && isInLobby && (
            <button
              className="w-full text-white rounded p-2 my-2 bg-green-600 hover:bg-green-700"
              onClick={handleStartGame}
            >
              Start game!
            </button>
          )}
          {!isPlayerHost && isInLobby && (
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
