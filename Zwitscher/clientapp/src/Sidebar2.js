import React from "react";
import './Sidebar2.css';
import SidebarOption from "./SidebarOption";
import HomeIcon from '@mui/icons-material/Home';
import GroupsIcon from '@mui/icons-material/Groups';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import FiberNewIcon from '@mui/icons-material/FiberNew';
import PermIdentityIcon from '@mui/icons-material/PermIdentity';
import Button from '@mui/material/Button';
import {Link, Switch, Route } from 'react-router-dom';
import { SidebarData } from "./SidebarData";

function Sidebar() {
  return (
    <div className="sidebar">
      <ul className="sidebarList">
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
      </ul>
    </div>
  );
}

export default Sidebar;
