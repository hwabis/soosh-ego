import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createGame, addPlayer } from '../services/ApiService';
import GithubIcon from '../assets/github-mark.svg?react';

const GameLobbyScreen = () => {
  const [playerName, setPlayerName] = useState("");
  const [enteredGameId, setEnteredGameId] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const navigate = useNavigate();

  const handleCreateGame = async () => {
    const newGameId = await createGame(playerName, error => setErrorMessage(error));
    if (newGameId) {
      await navigate(`/play?gameId=${encodeURIComponent(newGameId.value)}&playerName=${encodeURIComponent(playerName)}`);
    }
  }

  const handleJoinGame = async () => {
    const success = await addPlayer(enteredGameId, playerName, error => setErrorMessage(error));
    if (success) {
      await navigate(`/play?gameId=${encodeURIComponent(enteredGameId)}&playerName=${encodeURIComponent(playerName)}`);
    }
  }

  return (
    <div className="flex flex-col justify-center items-center h-screen bg-gray-100">
      <div className="p-6 bg-red-900 rounded space-y-4">
        <p className="text-lg font-bold text-white">Soosh Ego!</p>
        <a
          href="https://gamewright.com/pdfs/Rules/SushiGoTM-RULES.pdf"
          target="_blank"
          rel="noopener noreferrer"
          className="font-medium text-blue-200 dark:text-blue-100 hover:underline"
        >
          Rules
        </a>
        <div className="space-y-2">
          <label
            htmlFor="name-input"
            className="block font-medium text-white"
          >
            Your name:
          </label>
          <input
            id="name-input"
            type="text"
            value={playerName}
            onChange={e => setPlayerName(e.target.value)}
            className="block w-full rounded p-2"
          />
          <button
            onClick={() => void handleCreateGame()}
            className="w-full text-white rounded p-2 bg-green-600  hover:bg-green-700"
          >
            Create New Game
          </button>
        </div>
        <div className="space-y-2">
          <label
            htmlFor="game-id-input"
            className="block font-medium text-white"
          >
            Game ID:
          </label>
          <input
            id="game-id-input"
            type="text"
            value={enteredGameId}
            onChange={e => setEnteredGameId(e.target.value)}
            className="block w-full rounded p-2"
          />
          <button
            onClick={() => void handleJoinGame()}
            className="w-full text-white rounded p-2 bg-green-600  hover:bg-green-700"
          >
            Join Game
          </button>
        </div>
      </div>
      <p className="font-medium text-red-900 h-6 m-4">{errorMessage}</p>
      <div className="absolute bottom-8 right-8">
        <a
          href="https://github.com/hwabis/soosh-ego"
          target="_blank"
          rel="noopener noreferrer">
          <GithubIcon />
        </a>
      </div>
    </div>
  );
}

export default GameLobbyScreen;
