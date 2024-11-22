export interface GameId {
    value: string;
}

export const createGame = async (playerName: string): Promise<GameId> => {
    const response = await fetch(`${import.meta.env.VITE_SOOSH_EGO_API_URL}/api/gamelobby/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Value: playerName })
    });

    if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData);
    }

    return await response.json();
};
