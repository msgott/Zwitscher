import React, { useState, createContext, useEffect } from "react";
import { useLocation } from "react-router-dom";

import Header from "./Header";

import Sidebar2 from "./Sidebar2";
import Feed from "./Feed";
import Widgets from "./Widgets";
import "./AppZwitscher.css";



export const ThemeContext = createContext(null);


function AppZwitscher() {
  // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

  // Whenever a theme is inherited by some components (Trending, New etc.) The Value is passed through in this
  // Component. If we are in /Zwitscher for the first time, "light" default theme 
  // set the theme to 'light mode' in the beginning and have the opportunity to change theme
  // depending on toggleTheme

  const location = useLocation();
      //console.log(location);
      const screen = location.state?.screen;
      const initialTheme = screen !== undefined ? screen : 'light';


  const [theme, setTheme] = useState(initialTheme);
  const toggleTheme = () => {
    setTheme((curr) => (curr === "light" ? "dark" : "light"));
  };
    const [sessionData, setSessionData] = useState(null);

    useEffect(() => {
        const fetchUserSession = async () => {
            try {
                // Fetch session data
                var sessionResponse = await fetch("https://localhost:7160/Api/UserDetails");
                var sessionJsonData = await sessionResponse.json();
                setSessionData(sessionJsonData);


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }
        fetchUserSession();

    }, []);

  return (
    // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      <div className="beginning" id={theme}>
        <div className="Header-app">
          <Header />
        </div>
        <div className="app">
          <Sidebar2 value = {theme} />
          <Feed value = {theme} sessionData = {sessionData}/>
                  <Widgets />
        </div>
      </div>
    </ThemeContext.Provider>
  );
}

export default AppZwitscher;
