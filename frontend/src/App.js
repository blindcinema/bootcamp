import React, { useContext } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';
import Navbar from './components/Navbar';
import HeroText from './components/HeroText';
import ClientPackageReceived from './routes/ClientPackageReceved';
import { TestClient } from './routes/TestClient';
import { ThemeContext } from './context/ThemeContext';



function App() {
    const themeContext = useContext(ThemeContext);
    return (
        <>
            <Navbar />
            <HeroText />
            
            
            <footer>
                {/* Add footer content here */}
            </footer>
        </>
    );
}

export default App;
