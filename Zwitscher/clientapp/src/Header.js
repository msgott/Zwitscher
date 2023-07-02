import React, { useContext } from "react";
import { NavLink, Link } from "react-router-dom";
import "./Header.css";
import zwitscher_logo from "./zwitscher_logo.svg";
import ReactSwitch from "react-switch";
import LoginIcon from "@mui/icons-material/Login";
import { ThemeContext } from "./AppZwitscher"; // Access ThemeContext from App function to have toggle in the header

function Header() {
  const { theme, toggleTheme } = useContext(ThemeContext);

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
          <ReactSwitch onChange={toggleTheme} checked={theme === "dark"} />
        </div>

        <div className="Login_Icon">
          <a
            href="https://localhost:7160/Auth"
            style={{ textDecoration: "none" }}
          >
            <LoginIcon />
          </a>
        </div>

        {/*Hello button to figure out how to access Users2 data
        <button
          onClick={async () => {
            try {
              //Not working because Users2 Endpoint not implemented remotely
              const response = await fetch("/Users2");
              const data = await response.json();
              console.log(data[0].LastName);
              // Do something with the data here
              console.log(data);
            } catch (error) {
              console.error(error);
              // Handle the error here
            }
          }}
        >
          Hello
        </button>*/}
      </div>
    </div>
  );
}

export default Header;
