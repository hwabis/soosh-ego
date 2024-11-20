const GameLobby = () => {
  return (
    <div>
      <div>
        <label>Your name: </label>
        <input type="text" />
      </div>
      <button>Create New Game</button>
      <div>
        <label>Game ID: </label>
        <input type="text" />
      </div>
      <button>Join Game</button>
    </div>
  );
}

export default GameLobby;
