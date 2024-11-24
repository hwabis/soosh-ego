import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import GameLobbyScreen from './components/GameLobbyScreen';
import PlayScreen from './components/PlayScreen';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<GameLobbyScreen />} />
        <Route path="/play" element={<PlayScreen />} />
      </Routes>
    </Router>
  );
}

export default App;
