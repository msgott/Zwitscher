import React from "react";
import './Sidebar.css';
import SidebarOption from "./SidebarOption";
import HomeIcon from '@mui/icons-material/Home';
import GroupsIcon from '@mui/icons-material/Groups';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import FiberNewIcon from '@mui/icons-material/FiberNew';
import ListAltIcon from '@mui/icons-material/ListAlt';
import PermIdentityIcon from '@mui/icons-material/PermIdentity';
import Button from '@mui/material/Button';
import sidebarZwitscher_Icon from './Images/Zwitscher_Logo.png';
import { BrowserRouter as Router, Link, Switch, Route } from 'react-router-dom';





function Sidebar() {
    return (
      <div className="sidebar">
        <SidebarOption selected Icon={HomeIcon} text="Home" />
        <SidebarOption Icon={GroupsIcon} text="Public" />
        <SidebarOption Icon={TrendingUpIcon} text="Trending" />
        <SidebarOption Icon={FiberNewIcon} text="New" />
        <SidebarOption Icon={MailOutlineIcon} text="Messages" />
        <SidebarOption Icon={PermIdentityIcon} text="Profile" />

        <Button variant="outlined" className="sidebarButton" fullWidth>Zwitscher</Button>
      
      </div>
    );
  }
  
export default Sidebar;