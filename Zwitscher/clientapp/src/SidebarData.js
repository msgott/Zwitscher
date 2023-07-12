import React from "react";
import SidebarOption from "./SidebarOption";
import HomeIcon from "@mui/icons-material/Home";
import GroupsIcon from "@mui/icons-material/Groups";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import FiberNewIcon from "@mui/icons-material/FiberNew";
import ListAltIcon from "@mui/icons-material/ListAlt";


export const SidebarData = [
  {
    text: "Home",
    icon: <HomeIcon />,
    link: "/Zwitscher",
  },
  {
    text: "Public",
    icon: <GroupsIcon />,
    link: "/Zwitscher/public",
  },
  {
    text: "Trending",
    icon: <TrendingUpIcon />,
    link: "/Zwitscher/trending",
  }
  

];
