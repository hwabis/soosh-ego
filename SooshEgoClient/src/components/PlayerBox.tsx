import { Player } from "../models/Models";

interface PlayerBoxProps {
  player: Player;
  isLocalPlayer: boolean;
}

const PlayerBox = ({ player, isLocalPlayer }: PlayerBoxProps) => {
  return (
    <div className={`"bg-white border-2 ${isLocalPlayer ? "border-green-600" : "border-red-900"} p-4 rounded w-60"`}>
      <label className={`text-lg font-bold ${isLocalPlayer ? "text-green-600" : "text-red-900"}`}>
        {isLocalPlayer ? `${player.name.value} (You)` : player.name.value}
      </label>
      <div className="mt-2">
        <p className="font-medium">Played Cards:</p>
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
