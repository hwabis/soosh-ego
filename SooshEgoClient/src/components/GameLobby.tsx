import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { createGame } from '../services/ApiService';

const GameLobby = () => {
  const [playerName, setPlayerName] = useState('');
  // todo entered game id
  const [errorMessage, setErrorMessage] = useState('');

  const navigate = useNavigate();

  const handleCreateGame = async () => {
    try {
      const newGameId = await createGame(playerName);
      navigate(`/play/${newGameId.value}?playerName=${encodeURIComponent(playerName)}`);
    } catch (error) {
      setErrorMessage(String(error));
    }
  }

  return (
    <div>
      <div>
        <label>Your name: </label>
        <input type="text" value={playerName} onChange={(e) => setPlayerName(e.target.value)} />
      </div>
      <button onClick={handleCreateGame}>Create New Game</button>
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
