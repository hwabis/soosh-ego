import { Player } from "../models/Models";

interface PlayerBoxProps {
  player: Player;
  isLocalPlayer: boolean;
}

const PlayerBox = ({ player, isLocalPlayer }: PlayerBoxProps) => {
  return (
    <div className={`bg-white border-2 ${isLocalPlayer ? "border-green-600" : "border-red-900"} p-4 w-60 rounded`}>
      <p className="block text-lg font-bold">
        {isLocalPlayer ? `${player.name.value} (You)` : player.name.value}
      </p>
      <p className="block font-medium">
        {player.connectionId ? "[Connected]" : "[Disconnected]"}
      </p>
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
