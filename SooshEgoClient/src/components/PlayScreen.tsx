import { useParams } from "react-router-dom";

const PlayScreen = () => {
  const { gameId } = useParams();

  if (!gameId) {
    return (<p>Game ID is missing</p>); // todo
  }

  return (
    <div>
      <h1>Play Screen</h1>
      <p>Game ID: {gameId}</p>
    </div>
  );
}

export default PlayScreen;
