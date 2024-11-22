import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import GameLobby from './components/GameLobby';
import PlayScreen from './components/PlayScreen';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<GameLobby />} />
        <Route path="/play" element={<PlayScreen />} />
      </Routes>
    </Router>
  );
}

export default App;
