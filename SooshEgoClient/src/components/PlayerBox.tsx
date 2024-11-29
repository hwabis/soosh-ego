import { CARD_DETAILS } from "../models/CardAppearanceDetails";
import { Card, CardType, GameStage, Player } from "../models/Models";

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

  const sortCards = (cards: Card[]) => {
    const cardPriority: Record<CardType, number> = {
      [CardType.Tempura]: 0,
      [CardType.Sashimi]: 1,
      [CardType.Dumpling]: 2,
      [CardType.MakiRoll3]: 3,
      [CardType.MakiRoll2]: 3,
      [CardType.MakiRoll1]: 3,
      [CardType.SalmonNigiri]: 4,
      [CardType.SquidNigiri]: 4,
      [CardType.EggNigiri]: 4,
      [CardType.Wasabi]: 4,
      [CardType.Pudding]: 5,
      [CardType.Chopsticks]: 6,
    };

    const groupedCards: Record<number, Card[]> = {};

    cards.forEach(card => {
      const groupKey = cardPriority[card.cardType];
      if (!groupedCards[groupKey]) {
        groupedCards[groupKey] = [];
      }
      groupedCards[groupKey].push(card);
    });

    return Object.keys(groupedCards)
      .sort((a, b) => Number(a) - Number(b))
      .flatMap(key => groupedCards[Number(key)]);
  };

  const sortedCards = sortCards(player.cardsInPlay);

  return (
    <div className={`bg-white border-2 ${isLocalPlayer ? "border-green-600" : "border-red-900"} p-4 w-64 rounded`}>
      <p className="text-lg font-bold">
        {isLocalPlayer ? `${player.name.value} (You)` : player.name.value}
      </p>
      <p className="font-medium">
        {!player.connectionId && "[Disconnected]"}
      </p>
      <p className="font-medium">
        Points: {player.pointsAtEndOfPreviousRound}
      </p>
      <p className="font-medium">
        {getTurnStatus()}
      </p>
      <hr className="my-2" />
      <div className="mt-2">
        <div className="flex flex-col gap-1 overflow-y-auto min-h-80 max-h-80">
          {sortedCards.map((card, index) => {
            const details = CARD_DETAILS[card.cardType];
            return (
              <div
                key={index}
                className={`px-2 py-1 rounded ${details.bgColor} font-medium`}
              >
                {details.displayName} [{details.description}]
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
};

export default PlayerBox;
