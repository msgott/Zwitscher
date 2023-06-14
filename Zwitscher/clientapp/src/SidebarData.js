import React from 'react'
import SidebarOption from "./SidebarOption";
import HomeIcon from '@mui/icons-material/Home';
import GroupsIcon from '@mui/icons-material/Groups';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import FiberNewIcon from '@mui/icons-material/FiberNew';
import ListAltIcon from '@mui/icons-material/ListAlt';
import PermIdentityIcon from '@mui/icons-material/PermIdentity';
import DashboardIcon from '@mui/icons-material/Dashboard';

export const SidebarData = [
    {
        text: "Home",
        icon: <HomeIcon />,
        link: "/Zwitscher"
    },
    {
        text: "Public",
        icon: <GroupsIcon />,
        link: "/Zwitscher/public"
    },
    {
        text: "Trending",
        icon: <TrendingUpIcon />,
        link: "/Zwitscher/trending"
    },
    {
        text: "New",
        icon: <FiberNewIcon />,
        link: "/Zwitscher/new"
    },
    {
        text: "Messages",
        icon: <MailOutlineIcon />,
        link: "/Zwitscher/messages"
    },
    {
        text: "Profile",
        icon: <PermIdentityIcon />,
        link: "/Zwitscher/profile"
    },
    {
        text: "Dashboard",
        icon: <DashboardIcon />,
        link: "/Zwitscher/dashboard"
    },

]