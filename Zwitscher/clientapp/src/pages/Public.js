import React, {useState} from 'react'
import {Routes,Route,useNavigate,useLocation} from "react-router-dom";
import { ThemeContext } from "../AppZwitscher";
import Header from "../Header";
import Sidebar2 from "../Sidebar2";
import Feed from "../Feed";
import Widgets from "../Widgets";

function Public() {
  const location = useLocation();
    const screen = location.state?.screen;

    const [theme, setTheme] = useState(screen);
    console.log("theme is: " +theme)

    const toggleTheme = () => {
      setTheme((curr) => (curr === "light" ? "dark" : "light"));
    };
    
  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
    <div className="beginning" id={theme}>
      <div className="Header-app">
        <Header />
      </div>
      <div className="app">
        <Sidebar2 value = {theme} />
        <Feed />
        <Widgets />
      </div>
    </div>
  </ThemeContext.Provider>
  )
}

export default Public
