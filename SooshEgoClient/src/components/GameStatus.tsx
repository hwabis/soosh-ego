import { Game } from "../models/Models";

interface GameStatusProps {
  game: Game;
}

const GameStatus = ({ game }: GameStatusProps) => {
  return (
    <div className="absolute top-4 left-4 shadow-md p-4">
      <p >Game ID: {game.gameId.value}</p>
      <p >Stage: {game.gameStage}</p>
    </div>
  );
};

export default GameStatus;
