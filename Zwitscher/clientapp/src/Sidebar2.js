import React, { useContext, useEffect, useState } from "react";
import "./Sidebar2.css";

import PermIdentityIcon from "@mui/icons-material/PermIdentity";
import HomeIcon from "@mui/icons-material/Home";
import GroupsIcon from "@mui/icons-material/Groups";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import FiberNewIcon from "@mui/icons-material/FiberNew";

import { SidebarData } from "./SidebarData";
import DashboardIcon from "@mui/icons-material/Dashboard";
import HelpIcon from '@mui/icons-material/Help';
import { useNavigate, useLocation } from "react-router-dom";
import { ThemeContext } from "./AppZwitscher";

function Sidebar(theme) {
  // Get the current session data from the User whos online
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
    
    const navigate = useNavigate();
    const handleProfileClick = (username) => {
        // Check if the user is already on the profile page
        if (window.location.pathname === `/profile/${username}`) {
            // Reload the current profile page
            window.location.reload();
        } else {
            // Navigate to the desired profile page
            navigate(`/profile/${username}`,{ state: { screen: theme.value } });
            window.location.reload();
        }
    };
  // Function to check if user is a moderator or administrator
  const isModeratorOrAdmin = () => {
    return data.RoleName === "Moderator" || data.RoleName === "Administrator";
    };
    

    // Active Sidebar
    const location = useLocation();
    const currentUrl = location.pathname;


  return (
    <div className="sidebar">
      <ul className="sidebarList">
        {/*Access this file SidebarData and go through it with a map function to build the sidebar without dashboard*/}
       {/* {SidebarData.map((val, key) => {
           
          return (

            <li
              key={key}
              className="row"
              onClick={() => {
                window.location.pathname = val.link;
              }}
            >
              <div className="icon">{val.icon}</div>
              <div className="text">{val.text}</div>
            </li>
            
          );
        })}
            */}

              {/*Home*/}
              
        <li
          className={`row ${currentUrl === '/' ? 'active' : ''}`}
          onClick={() => { navigate('/', {state: { screen: theme.value}})}}
        >
          <div className="icon">
            <HomeIcon />
          </div>
          <div className="text">Home</div>
        </li>

              {/*�ffentlich*/}
              {data.Username !== "" && (
        <li
          className={`row ${currentUrl === '/public' ? 'active' : ''}`}
          onClick={() => { navigate('/public', {state: { screen: theme.value}})}}
        >
          <div className="icon">
            <GroupsIcon />
          </div>
          <div className="text">Oeffentlich</div>
        </li>
              )}
        {/*Im Trend*/}
        {/*pass current theme in different page - theme.value default 'light'*/}
        <li
            className={`row ${currentUrl === '/trending' ? 'active' : ''}`}
            onClick={() => { navigate('/trending', {state: { screen: theme.value}})}}
          >
          <div className="icon">
              <TrendingUpIcon />
            </div>
            <div className="text">Im Trend</div>
          </li>

        {/*Aktuell*/}
        {/*pass current theme in different page - theme.value default 'light'*/}
        <li
            className={`row ${currentUrl === '/new' ? 'active' : ''}`}
            onClick={() => { navigate('/new', {state: { screen: theme.value}})}}
          >
          <div className="icon">
              <FiberNewIcon />
            </div>
            <div className="text">Aktuell</div>
          </li>  


        {/*Profile only visibale, if registered*/}
        {data.Username !== "" && (
                  <li

                      className={`row ${currentUrl === '/profile/'+ data.userID ? 'active' : ''}`}
                      onClick={() => { handleProfileClick(data.userID) }}

          >
            <div className="icon">
              <PermIdentityIcon />
            </div>
            <div className="text">Profil</div>
          </li>
        )}

        {/*Dashboard only visible, if the role is either Admin or Mod*/}
        {isModeratorOrAdmin() && (
          <li
            className={`row ${currentUrl === '/dashboard' ? 'active' : ''}`}
            onClick={() => {
              window.location.pathname = "/";
            }}
          >
            <div className="icon">
              <DashboardIcon />
            </div>
            <div className="text">Verwaltung</div>
          </li>
              )}
        {/*Support visable for all*/}      
        <li 
          className="row"
          onClick={() => {
            window.location.href = 'mailto:support@Zwitscher.de';
          }}>
            <div className="icon">
              <HelpIcon />
            </div>
            <div className="text">Support</div>
          </li>  
      
      </ul>
    </div>
  );
}

export default Sidebar;
