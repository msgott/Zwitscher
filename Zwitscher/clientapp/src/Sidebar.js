import React from "react";
import './Sidebar.css';
import TwitterIcon from '@mui/icons-material/Twitter';
import SidebarOption from "./SidebarOption";
import HomeIcon from '@mui/icons-material/Home';
import SearchIcon from '@mui/icons-material/Search';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import BookmarkBorderIcon from '@mui/icons-material/BookmarkBorder';
import ListAltIcon from '@mui/icons-material/ListAlt';
import PermIdentityIcon from '@mui/icons-material/PermIdentity';
import MoreHorizIcon from '@mui/icons-material/MoreHoriz';
import Button from '@mui/material/Button';
import sidebarZwitscher_Icon from './Images/Zwitscher_Logo.png';



function Sidebar() {
    return (
      <div className="sidebar">
        <div className="sidebarZwitscher_Icon">
          <img src={sidebarZwitscher_Icon} alt="sidebarZwitscher_Icon"/>
        </div>
        


        

        <SidebarOption selected Icon={HomeIcon} text="Home" />
        <SidebarOption Icon={SearchIcon} text="Search" />
        <SidebarOption Icon={NotificationsNoneIcon} text="Notifications" />
        <SidebarOption Icon={MailOutlineIcon} text="Messages" />
        <SidebarOption Icon={BookmarkBorderIcon} text="Bookmarks" />
        <SidebarOption Icon={ListAltIcon} text="Lists" />
        <SidebarOption Icon={PermIdentityIcon} text="Profile" />
        <SidebarOption Icon={MoreHorizIcon} text="More" />

        <Button variant="outlined" className="sidebarZwitscher" fullWidth>Zwitscher</Button>
      
      </div>
    );
  }
  
export default Sidebar;