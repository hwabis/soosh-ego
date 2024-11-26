import { GameStage, Player } from "../models/Models";

interface PlayerBoxProps {
  player: Player;
  isLocalPlayer: boolean;
  gameStage: GameStage;
}

const PlayerBox = ({ player, isLocalPlayer, gameStage }: PlayerBoxProps) => {
  const getTurnStatus = () => {
    if (gameStage !== GameStage.Playing) {
      return null;
    }
    return player.finishedTurn ? "Card chosen!" : "Choosing a card...";
  };

  return (
    <div className={`bg-white border-2 ${isLocalPlayer ? "border-green-600" : "border-red-900"} p-4 w-56 rounded`}>
      <p className="block text-lg font-bold">
        {isLocalPlayer ? `${player.name.value} (You)` : player.name.value}
      </p>
      <p className="block font-medium">
        {!player.connectionId && "[Disconnected]"}
      </p>
      <p className="block font-medium">
        {getTurnStatus()}
      </p>
      <div className="mt-2">
        <p className="font-medium">Played cards:</p>
        <ul className="list-disc list-inside">
          {player.cardsInPlay.map((card, index) => (
            <li key={index}>{card.cardType}</li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default PlayerBox;
