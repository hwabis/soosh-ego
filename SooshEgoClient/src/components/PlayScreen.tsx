import { useParams, useSearchParams } from "react-router-dom";

const PlayScreen = () => {
  const { gameId } = useParams();
  const [searchParams] = useSearchParams();
  const playerName = searchParams.get("playerName");

  if (!gameId || !playerName) {
    return (<p>Game ID or player name is missing from URL!</p>);
  }

  return (
    <div>
      <h1>Play Screen</h1>
      <p>Game ID: {gameId}</p>
      <p>Player Name: {playerName}</p>
    </div>
  );
}

export default PlayScreen;
