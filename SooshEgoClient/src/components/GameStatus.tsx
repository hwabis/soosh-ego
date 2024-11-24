import { Game } from "../models/Models";

interface GameStatusProps {
  game: Game;
  errorMessage: string;
}

const GameStatus = ({ game, errorMessage }: GameStatusProps) => {
  return (
    <div>
      <div className="absolute top-4 left-4">
        <div className="bg-red-900 rounded w-52 p-4">
          <label className="block font-medium text-white">Game ID: {game.gameId.value}</label>
          <label className="block font-medium text-white">Stage: {game.gameStage}</label>
        </div>
        <label className="block font-medium text-red-900 my-2">
          {errorMessage}
        </label>
      </div>
    </div>
  ); // todo start game button... becomes disabled when gamestage is started
};

export default GameStatus;
