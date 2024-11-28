import { useState } from "react";
import { Card } from "../models/Models";

interface CardSelectionProps {
  cards: Card[];
  selectionLimit: number;
  playerFinishedTurn: boolean;
  onConfirm: (selectedIndices: number[]) => void;
}

const CardSelection = ({ cards, selectionLimit, playerFinishedTurn, onConfirm, }: CardSelectionProps) => {
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
        {cards.map((card, index) => (
          <div
            key={index}
            className={`w-28 h-40 rounded p-2 cursor-pointer
              ${selectedIndices.includes(index) ? "bg-slate-600" : "bg-slate-300"}`}
            onClick={() => toggleCardSelection(index)}
          >
            <p className={`text-center font-medium ${selectedIndices.includes(index) ? "text-white" : "text-black"}`}
            >
              {card.cardType}
            </p>
          </div>
        ))}
      </div>
      {cards.length > 0 &&
        <button
          className="w-64 h-16 text-white text-lg font-medium p-4 ml-2 rounded bg-green-600 hover:bg-green-700"
          disabled={playerFinishedTurn}
          onClick={handleConfirm}
        >
          {playerFinishedTurn ? `Waiting for other players...` : `Go!`}
        </button>
      }
    </div>
  );
};

export default CardSelection;
