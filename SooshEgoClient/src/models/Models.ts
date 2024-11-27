// Everything here mirrors the models on the backend

export interface Game {
    gameId: GameId;
    gameStage: GameStage;
    numberOfRoundsCompleted: number;
    players: Player[];
    winnerName: string | null;
}

export interface GameId {
    value: string;
}

export enum GameStage {
    Waiting = "Waiting",
    Playing = "Playing",
    Finished = "Finished",
}

export interface Player {
    name: PlayerName;
    cardsInHand: Card[];
    cardsInPlay: Card[];
    finishedThisTurn: boolean;
    pointsAtEndOfPreviousRound: number;
    connectionId: string | null;
}

export interface PlayerName {
    value: string;
}

export interface Card {
    cardType: CardType;
}

export enum CardType {
    Tempura = "Tempura",
    Sashimi = "Sashimi",
    Dumpling = "Dumpling",
    MakiRoll1 = "MakiRoll1",
    MakiRoll2 = "MakiRoll2",
    MakiRoll3 = "MakiRoll3",
    SalmonNigiri = "SalmonNigiri",
    SquidNigiri = "SquidNigiri",
    EggNigiri = "EggNigiri",
    Wasabi = "Wasabi",
    Pudding = "Pudding",
    Chopsticks = "Chopsticks",
}
