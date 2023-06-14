import React, { useState, createContext, useEffect } from 'react';
import ReactSwitch from 'react-switch'
import { BrowserRouter, Link, Switch, Routes, Route, NavLink, useNavigate } from 'react-router-dom';
import { useLocation } from 'react-router-dom';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import Header from './Header';
import Sidebar from './Sidebar';
import Sidebar2 from './Sidebar2';
import Feed from './Feed';
import Widgets from './Widgets';
import './AppZwitscher.css';
import Profile from './pages/Profile'
import HomeIcon from '@mui/icons-material/Home';

export const ThemeContext = createContext(null);
export const goToProfileContext = createContext(null);

function AppZwitscher() {
  const [theme, setTheme] = useState('light');

  const toggleTheme = () => {
    setTheme((curr) => (curr === 'light' ? 'dark' : 'light'));
  };

  const [goToProfile, setGoToProfile] = useState(false);
  const navigate = useNavigate();


  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      <goToProfileContext.Provider value={{goToProfile, setGoToProfile}}>
      <div className="beginning" id={theme}>
        <div className="Header-app">
          <Header />
        </div>
        <div className="app">
          <Sidebar2 />
          <Feed />
        </div>
        <Routes>
          <Route path="/profile" element={<Profile />} />
        </Routes>
      </div>
      </goToProfileContext.Provider>
    </ThemeContext.Provider>
  );
}

export default AppZwitscher;
