import { CardType } from "./Models";

export const CARD_DETAILS: Record<CardType, {
    displayName: string;
    description: string;
    image: string;
    bgColor: string;
}> = {
    [CardType.Tempura]: {
        displayName: "Tempura",
        description: "x2 = 5",
        image: "/tempura.png",
        bgColor: "bg-purple-300"
    },
    [CardType.Sashimi]: {
        displayName: "Sashimi",
        description: "x3 = 10",
        image: "/sashimi.png",
        bgColor: "bg-lime-300"
    },
    [CardType.Dumpling]: {
        displayName: "Dumpling",
        description: "1, 3, 6, 10, 15",
        image: "/dumpling.png",
        bgColor: "bg-blue-300"
    },
    [CardType.MakiRoll3]: {
        displayName: "Maki Roll (3)",
        description: "Win 6/3",
        image: "/maki3.png",
        bgColor: "bg-red-500"
    },
    [CardType.MakiRoll2]: {
        displayName: "Maki Roll (2)",
        description: "Win 6/3",
        image: "/maki2.png",
        bgColor: "bg-red-500"
    },
    [CardType.MakiRoll1]: {
        displayName: "Maki Roll (1)",
        description: "Win 6/3",
        image: "/maki1.png",
        bgColor: "bg-red-500"
    },
    [CardType.SquidNigiri]: {
        displayName: "Squid Nigiri",
        description: "3",
        image: "/squid-nigiri.png",
        bgColor: "bg-yellow-300"
    },
    [CardType.SalmonNigiri]: {
        displayName: "Salmon Nigiri",
        description: "2",
        image: "/salmon-nigiri.png",
        bgColor: "bg-yellow-300"
    },
    [CardType.EggNigiri]: {
        displayName: "Egg Nigiri",
        description: "1",
        image: "/egg-nigiri.png",
        bgColor: "bg-yellow-300"
    },
    [CardType.Wasabi]: {
        displayName: "Wasabi",
        description: "Next Nigiri x3",
        image: "/wasabi.png",
        bgColor: "bg-yellow-300"
    },
    [CardType.Pudding]: {
        displayName: "Pudding",
        description: "End 6/-6",
        image: "/pudding.png",
        bgColor: "bg-pink-200"
    },
    [CardType.Chopsticks]: {
        displayName: "Scroll",
        description: "Swap for 2",
        image: "/chopsticks.png",
        bgColor: "bg-sky-200"
    },
};
