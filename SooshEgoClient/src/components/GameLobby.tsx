import { useState } from "react";
import { useNavigate } from "react-router-dom";

const GameLobby = () => {
  const [playerName, setPlayerName] = useState('');
  const [createdGameId, setCreatedGameId] = useState('');
  // todo entered game id
  const [errorMessage, setErrorMessage] = useState('');

  const navigate = useNavigate();

  const handleCreateGame = async () => {
    try {
      const response = await fetch('https://localhost:5001/api/gamelobby/create', { // todo local, production, environment variables ???
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Value: playerName })
      });

      if (!response.ok) {
        const errorData = await response.text();
        setErrorMessage(errorData);
        return;
      }

      const newGameId = await response.json();
      setCreatedGameId(newGameId);
      setErrorMessage('');
      navigate(`/play/${newGameId}`);
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
