import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { createGame, addPlayer } from '../services/ApiService';

const GameLobby = () => {
  const [playerName, setPlayerName] = useState('');
  const [enteredGameId, setEnteredGameId] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const navigate = useNavigate();

  const handleCreateGame = async () => {
    try {
      const newGameId = await createGame(playerName);
      navigate(`/play?gameId=${encodeURIComponent(newGameId.value)}&playerName=${encodeURIComponent(playerName)}`);
    } catch (error) {
      setErrorMessage(String(error));
    }
  }

  const handleJoinGame = async () => {
    try {
      await addPlayer(enteredGameId, playerName);
      navigate(`/play?gameId=${encodeURIComponent(enteredGameId)}&playerName=${encodeURIComponent(playerName)}`);
    } catch (error) {
      setErrorMessage(String(error));
    }
  }

  return (
    <div>
      <div>
        <label>Your name: </label>
        <input type="text" value={playerName} onChange={e => setPlayerName(e.target.value)} />
      </div>
      <button onClick={handleCreateGame}>Create New Game</button>
      <div>
        <label>Game ID: </label>
        <input type="text" value={enteredGameId} onChange={e => setEnteredGameId(e.target.value)} />
      </div>
      <button onClick={handleJoinGame}>Join Game</button>
      <div>{errorMessage}</div>
    </div>
  );
}

export default GameLobby;
