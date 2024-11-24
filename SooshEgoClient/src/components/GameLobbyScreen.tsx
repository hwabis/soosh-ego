import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { createGame, addPlayer } from '../services/GameLobbyApiService';

const GameLobbyScreen = () => {
  const [playerName, setPlayerName] = useState('');
  const [enteredGameId, setEnteredGameId] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const navigate = useNavigate();

  const handleCreateGame = async () => {
    const newGameId = await createGame(playerName, error => setErrorMessage(error));
    if (newGameId) {
      navigate(`/play?gameId=${encodeURIComponent(newGameId.value)}&playerName=${encodeURIComponent(playerName)}`);
    }
  }

  const handleJoinGame = async () => {
    const success = await addPlayer(enteredGameId, playerName, error => setErrorMessage(error));
    if (success) {
      navigate(`/play?gameId=${encodeURIComponent(enteredGameId)}&playerName=${encodeURIComponent(playerName)}`);
    }
  }

  return (
    <div className="flex flex-col justify-center items-center h-screen bg-gray-100">
      <div className="p-6 bg-red-900 rounded space-y-4">
        <label className="text-lg font-bold text-white">Sushi Go!</label>
        <div className="space-y-2">
          <label className="block font-medium text-white">Your name:</label>
          <input
            type="text"
            value={playerName}
            onChange={(e) => setPlayerName(e.target.value)}
            className="block w-full rounded p-2"
          />
          <button
            onClick={handleCreateGame}
            className="w-full text-white rounded p-2 bg-green-600  hover:bg-green-700"
          >
            Create New Game
          </button>
        </div>
        <div className="space-y-2">
          <label className="block font-medium text-white">Game ID:</label>
          <input
            type="text"
            value={enteredGameId}
            onChange={(e) => setEnteredGameId(e.target.value)}
            className="block w-full rounded p-2"
          />
          <button
            onClick={handleJoinGame}
            className="w-full text-white rounded p-2 bg-green-600  hover:bg-green-700"
          >
            Join Game
          </button>
        </div>
      </div>
      <div className="block font-medium text-red-900 h-6 m-4">{errorMessage}</div>
    </div>
  );
}

export default GameLobbyScreen;
