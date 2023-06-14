import React, { useContext } from 'react';
import { NavLink, Link } from 'react-router-dom';
import './Header.css';
import zwitscher_logo from './zwitscher_logo.svg';
import ReactSwitch from 'react-switch';
import LoginIcon from '@mui/icons-material/Login';
import { ThemeContext } from './AppZwitscher';  // Access ThemeContext from App function to have toggle in the header

function Header() {
  const { theme, toggleTheme } = useContext(ThemeContext);

  return (
    <div className="Header">
      <div className="Logo">
        <img src={zwitscher_logo} alt="Icon" text="Zwitscher" />
        <div className="Logo_Text" style={{width:"10%"}}>
          <span>Zwitscher</span>
        </div>
      </div>
      <div className="Login-Toggle">
      <div className="Login_Icon">
        <LoginIcon style={{ fontSize: '24px' }} />
      </div>
      <div className="toggle">
        <ReactSwitch
          onChange={toggleTheme}
          checked={theme === 'dark'}
        />
      </div>

      {/*Hello button to figure out how to access Users2 data*/}
      <button onClick={async () => {
    try {
        const response = await fetch('/Users2');
        const data = await response.json();
        console.log(data[0].LastName);
        // Do something with the data here
        console.log(data);
    } catch (error) {
        console.error(error);
        // Handle the error here
    }
}}>Hello</button>



    </div>
    </div>
  );
}

export default Header;
