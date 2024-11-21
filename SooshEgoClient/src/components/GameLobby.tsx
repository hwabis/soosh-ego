import { useState } from "react";

const GameLobby = () => {
  const [createdGameId, setCreatedGameId] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const createGame = async () => {
    try {
      const response = await fetch('https://localhost:5001/GameLobby/create', { // todo local vs env links ???
        method: 'POST'
      });

      if (!response.ok) {
        const errorData = await response.text();
        setErrorMessage(errorData);
        return;
      }

      const newGameId = await response.json();
      setCreatedGameId(newGameId);
      setErrorMessage('');
      console.log(newGameId); //
      // todo go to play screen using createdGameId, on play screen connect websocket and join
    } catch (error) {
      setErrorMessage(String(error));
    }
  }

  return (
    <div>
      <div>
        <label>Your name: </label>
        <input type="text" />
      </div>
      <button onClick={createGame}>Create New Game</button>
      <div>
        <label>Game ID: </label>
        <input type="text" />
      </div>
      <button>Join Game</button>
      <div>{errorMessage}</div>
    </div>
  );
}

export default GameLobby;
