import React, {useState} from "react";
import {BrowserRouter,Routes,Route,Link } from "react-router-dom";
import AppZwitscher from "./AppZwitscher";
import Profile from "./pages/Profile";
import Dashboard from "./pages/Dashboard";
import New from "./pages/New";
import Public from "./pages/Public";
import Trending from "./pages/Trending";
import NotFound from "./NotFound";

function App() {
  // App function is above AppZwitscher so that routing will function for all componants
  // from AppZwitscher component on (Sidebar, Feed etc.)

  return (
    <div>
    <Routes>
      <Route path="/" element={<AppZwitscher />} />
      <Route path="public" element={<Public />} />
      <Route path="trending" element={<Trending />} />
      <Route path="new" element={<New />} />
      <Route path="profile/*" element={<Profile />} />
      <Route path="dashboard" element={<Dashboard />} />
      <Route path="/error" element={<h1>Error 404</h1>} />
      <Route path="*" element={<NotFound />} />
    </Routes>
    </div>
  );
}

export default App;
