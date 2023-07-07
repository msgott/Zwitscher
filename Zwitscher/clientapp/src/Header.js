import React, { useContext, useEffect, useState } from "react";
import { NavLink, Link } from "react-router-dom";
import "./Header.css";
import zwitscher_logo from "./zwitscher_logo.svg";
import ReactSwitch from "react-switch";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import LoginIcon from "@mui/icons-material/Login";
import { ThemeContext } from "./AppZwitscher"; // Access ThemeContext from App function to have toggle in the header

function Header() {
  const { theme, toggleTheme } = useContext(ThemeContext);

  // Get authorization data from backend
  const [data, setData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("https://localhost:7160/Api/UserDetails"); // Replace with your API endpoint
        const jsonData = await response.json();
        setData(jsonData);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);

  // Function to check if user is a moderator or administrator
  const isModeratorOrAdmin = () => {
    return data.RoleName === "Moderator" || data.RoleName === "Administrator";
  };

  return (
    <div className="Header">
      <div className="Header_Left">
        <Link to="/" style={{ textDecoration: "none" }}>
          <div className="Logo">
            <div className="Logo_Zwitscher">
              <img src={zwitscher_logo} alt="Icon" text="Zwitscher" />
            </div>
            <div className="Logo_Text">
              <span>Zwitscher</span>
            </div>
          </div>
        </Link>
      </div>
      <div className="Header_Middle"></div>
      <div className="Header_Right">
        <div className="toggle">
          {theme === "dark" ? (
            <LightModeIcon onClick={toggleTheme} />
          ) : (
            <DarkModeIcon onClick={toggleTheme} />
          )}
        </div>

        <div className="Login_Icon">
          <LoginIcon
            onClick={() =>
              (window.location.href = "https://localhost:7160/Auth")
            }
          />
        </div>
      </div>
    </div>
  );
}

export default Header;
