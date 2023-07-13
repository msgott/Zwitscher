import React, {useEffect, useState} from 'react'
import {Routes,Route,useNavigate,useLocation} from "react-router-dom";
import { ThemeContext } from "../AppZwitscher";
import Header from "../Header";
import Sidebar2 from "../Sidebar2";
import Feed from "../Feed";
import Widgets from "../Widgets";

function Trending() {
  const location = useLocation();
      console.log(location);
      const screen = location.state?.screen;

      const [theme, setTheme] = useState(screen);
      //console.log("theme is: " +theme)

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
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
    <div className="beginning" id={theme}>
      <div className="Header-app">
        <Header />
      </div>
      <div className="app">
        <Sidebar2 value = {theme} />
                  <Feed sessionData={sessionData}/>
        <Widgets />
      </div>
    </div>
  </ThemeContext.Provider>
  )
}

export default Trending
