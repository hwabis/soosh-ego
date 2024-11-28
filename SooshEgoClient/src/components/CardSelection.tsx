import { useState } from "react";
import { CardType, Player } from "../models/Models";

const CARD_DETAILS: Record<CardType, {
  name: string;
  description: string;
  image: string;
  bgColor: string;
}> = {
  [CardType.Tempura]: {
    name: "Tempura",
    description: "x2 = 5",
    image: "/tempura.png",
    bgColor: "bg-purple-300"
  },
  [CardType.Sashimi]: {
    name: "Sashimi",
    description: "x3 = 10",
    image: "/sashimi.png",
    bgColor: "bg-lime-300"
  },
  [CardType.Dumpling]: {
    name: "Dumpling",
    description: "1 3 6 10 15",
    image: "/dumpling.png",
    bgColor: "bg-blue-300"
  },
  [CardType.MakiRoll3]: {
    name: "Maki Roll (3)",
    description: "Most 6/3",
    image: "/maki3.png",
    bgColor: "bg-red-500"
  },
  [CardType.MakiRoll2]: {
    name: "Maki Roll (2)",
    description: "Most 6/3",
    image: "/maki2.png",
    bgColor: "bg-red-500"
  },
  [CardType.MakiRoll1]: {
    name: "Maki Roll (1)",
    description: "Most 6/3",
    image: "/maki1.png",
    bgColor: "bg-red-500"
  },
  [CardType.SalmonNigiri]: {
    name: "Salmon Nigiri",
    description: "3",
    image: "/salmon-nigiri.png",
    bgColor: "bg-yellow-300"
  },
  [CardType.SquidNigiri]: {
    name: "Squid Nigiri",
    description: "2",
    image: "/squid-nigiri.png",
    bgColor: "bg-yellow-300"
  },
  [CardType.EggNigiri]: {
    name: "Egg Nigiri",
    description: "1",
    image: "/egg-nigiri.png",
    bgColor: "bg-yellow-300"
  },
  [CardType.Wasabi]: {
    name: "Wasabi",
    description: "Next Nigiri x3",
    image: "/wasabi.png",
    bgColor: "bg-yellow-300"
  },
  [CardType.Pudding]: {
    name: "Pudding",
    description: "End 6/-6",
    image: "/pudding.png",
    bgColor: "bg-pink-200"
  },
  [CardType.Chopsticks]: {
    name: "Scroll",
    description: "Swap for 2",
    image: "/chopsticks.png",
    bgColor: "bg-sky-200"
  },
};

interface CardSelectionProps {
  localPlayer: Player;
  selectionLimit: number;
  onConfirm: (selectedIndices: number[]) => void;
}

const CardSelection = ({ localPlayer, selectionLimit, onConfirm, }: CardSelectionProps) => {
  const [selectedIndices, setSelectedIndices] = useState<number[]>([]);

  const toggleCardSelection = (selectedIndex: number) => {
    setSelectedIndices(prev => {
      if (prev.includes(selectedIndex)) {
        // Deselect
        return prev.filter(i => i !== selectedIndex);
      } else if (prev.length < selectionLimit) {
        // Select
        return [...prev, selectedIndex];
      }

      return prev;
    });
  };

  const handleConfirm = () => {
    onConfirm(selectedIndices);
    setSelectedIndices([]);
  };

  return (
    <div className="flex flex-col justify-center items-center gap-2">
      <div className="flex flex-wrap gap-2">
        {localPlayer.cardsInHand.map((card, index) => {
          const details = CARD_DETAILS[card.cardType];
          return (
            <div
              key={index}
              className={`w-28 h-36 rounded p-1 cursor-pointer flex flex-col items-center justify-between
                ${details.bgColor} ${selectedIndices.includes(index) ? "ring-4 ring-cyan-700" : ""}`}
              onClick={() => toggleCardSelection(index)}
            >
              <p className="text-center font-bold">{details.name}</p>
              <img
                src={details.image}
                alt={details.name}
                className="m-4 h-full max-h-full object-contain overflow-hidden"
              />
              <p className="text-center font-medium text-sm">{details.description}</p>
            </div>);
        })}
      </div>
      {
        localPlayer.cardsInHand.length > 0 &&
        <button
          className="w-64 h-16 text-white text-lg font-medium p-4 ml-2 rounded bg-green-600 hover:bg-green-700"
          disabled={localPlayer.finishedTurn}
          onClick={handleConfirm}
        >
          {localPlayer.finishedTurn
            ? "Waiting for other players..."
            : `${localPlayer.cardsInPlay.some(card => card.cardType === CardType.Chopsticks) ? "Go! (Scroll available)" : "Go!"}`
          }
        </button>
      }
    </div >
  );
};

export default CardSelection;
