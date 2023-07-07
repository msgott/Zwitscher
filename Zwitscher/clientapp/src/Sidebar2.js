import React, { useEffect, useState } from "react";
import "./Sidebar2.css";
import SidebarOption from "./SidebarOption";
import HomeIcon from "@mui/icons-material/Home";
import GroupsIcon from "@mui/icons-material/Groups";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import MailOutlineIcon from "@mui/icons-material/MailOutline";
import FiberNewIcon from "@mui/icons-material/FiberNew";
import PermIdentityIcon from "@mui/icons-material/PermIdentity";
import Button from "@mui/material/Button";
import { Link, Switch, Route } from "react-router-dom";
import { SidebarData } from "./SidebarData";
import DashboardIcon from "@mui/icons-material/Dashboard";

function Sidebar() {
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
  // Function to check if user is a moderator or administrator
  const isModeratorOrAdmin = () => {
    return data.RoleName === "Moderator" || data.RoleName === "Administrator";
  };

  return (
    <div className="sidebar">
      <ul className="sidebarList">
        {/*Access this file SidebarData and go through it with a map function to build the sidebar without dashboard*/}
        {SidebarData.map((val, key) => {
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

        {/*Profile only visibale, if registered*/}
        {data.Username !== "" && (
          <li
            className="row"
            onClick={() => {
              window.location.pathname = "/Zwitscher/profile";
            }}
          >
            <div className="icon">
              <PermIdentityIcon />
            </div>
            <div className="text">Profile</div>
          </li>
        )}

        {/*Dashboard only visibale, if the role is either Admin or Mod*/}
        {isModeratorOrAdmin() && (
          <li
            className="row"
            onClick={() => {
              window.location.pathname = "/";
            }}
          >
            <div className="icon">
              <DashboardIcon />
            </div>
            <div className="text">Dashboard</div>
          </li>
        )}
      </ul>
    </div>
  );
}

export default Sidebar;
