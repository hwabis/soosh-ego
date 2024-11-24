import { GameId } from "../models/Models";

export const createGame = async (
    playerName: string,
    onError: (error: string) => void): Promise<GameId | null> => {
    const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/gamelobby/create`, {
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

    return await response.json();
};

export const addPlayer = async (
    gameId: string,
    playerName: string,
    onError: (error: string) => void): Promise<boolean> => {
    const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/gamelobby/add-player`, {
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
};

export const startGame = async (
    gameId: string,
    onError: (error: string) => void): Promise<boolean> => {
    const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/gamelobby/start-game`, {
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
};
