import { GameId } from "../models/Models";

export const createGame = async (
    playerName: string,
    onError: (error: string) => void): Promise<GameId | null> => {
    try {
        const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                value: playerName
            })
        });

        if (!response.ok) {
            const errorData = await response.text();
            onError(errorData);
            return null;
        }

        // todo https://www.reddit.com/r/typescript/comments/18im04a/how_do_i_satisfy_this_type_requirement_no_matter/
        return await response.json() as GameId;
    }
    catch {
        onError("Failed to connect to the server.");
        return null;
    }
};

export const addPlayer = async (
    gameId: string,
    playerName: string,
    onError: (error: string) => void): Promise<boolean> => {
    try {
        const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/add-player`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                GameId: { value: gameId },
                PlayerName: { value: playerName }
            })
        });

        if (!response.ok) {
            const errorData = await response.text();
            onError(errorData);
            return false;
        }

        return true;
    }
    catch {
        onError("Failed to connect to the server.");
        return false;
    }
};

export const startGame = async (
    gameId: string,
    onError: (error: string) => void): Promise<boolean> => {
    try {
        const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/start-game`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                value: gameId
            })
        });

        if (!response.ok) {
            const errorData = await response.text();
            onError(errorData);
            return false;
        }

        return true;
    } catch {
        onError("Failed to connect to the server.");
        return false;
    }
};

export const playCard = async (
    gameId: string,
    playerName: string,
    card1Index: number,
    card2Index: number,
    onError: (error: string) => void): Promise<boolean> => {
    try {
        const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/play-card`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                GameId: { value: gameId },
                PlayerName: { value: playerName },
                Card1Index: card1Index,
                Card2Index: card2Index,
            })
        });

        if (!response.ok) {
            const errorData = await response.text();
            onError(errorData);
            return false;
        }

        return true;
    }
    catch {
        onError("Failed to connect to the server.");
        return false;
    }
};
