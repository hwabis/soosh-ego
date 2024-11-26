import React, { useState } from "react";
import { Card } from "../models/Models";

interface CardSelectionProps {
  cards: Card[];
  selectionLimit: number;
  onConfirm: (selectedIndices: number[]) => void;
}

const CardSelection: React.FC<CardSelectionProps> = ({
  cards,
  selectionLimit,
  onConfirm,
}) => {
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
            className={`w-28 h-40 border-4 rounded p-2 cursor-pointer
              ${selectedIndices.includes(index) ? "border-blue-300" : "border-gray-500"}`}
            onClick={() => toggleCardSelection(index)}
          >
            <p className="text-center font-bold">{card.cardType}</p>
          </div>
        ))}
      </div>
      {cards.length > 0 &&
        <button
          className="w-32 text-white text-lg font-medium p-4 ml-2 rounded bg-green-600 hover:bg-green-700"
          onClick={handleConfirm}
        >
          Confirm!
        </button>
      }

    </div>
  );
};

export default CardSelection;
