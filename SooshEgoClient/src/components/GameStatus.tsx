import { Game } from "../models/Models";

interface GameStatusProps {
  game: Game;
}

const GameStatus = ({ game }: GameStatusProps) => {
  return (
    <div className="absolute top-4 left-4 p-4 bg-red-900 rounded">
      <label className="block font-medium text-white">Game ID: {game.gameId.value}</label>
      <label className="block font-medium text-white">Stage: {game.gameStage}</label>
    </div>
  ); // todo start game button... becomes disabled when gamestage is started
};

export default GameStatus;
