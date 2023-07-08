import React, { useState, createContext, useEffect, useContext } from "react";
import ReactSwitch from "react-switch";
import {
  BrowserRouter,
  Link,
  Switch,
  Routes,
  Route,
  NavLink,
  useNavigate,
} from "react-router-dom";
import { useLocation } from "react-router-dom";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import Header from "../Header";
import Sidebar2 from "../Sidebar2";
import Feed from "../Feed";
import Widgets from "../Widgets";
import "./Profile.css";
import { ThemeContext } from "../AppZwitscher";

import Button from "@mui/material/Button";
import HomeIcon from "@mui/icons-material/Home";

export const goToProfileContext = createContext(null);

function Profile() {
  // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

  // set the theme to 'light mode' in the beginning and have the opportunity to change theme
  // depending on toggleTheme
  const [theme, setTheme] = useState("light");

  const toggleTheme = () => {
    setTheme((curr) => (curr === "light" ? "dark" : "light"));
  };
  // Navigate to the profile page if set to true. Follow goToProfileContext.Provider to understand
  // routing with React v18

  const [goToProfile, setGoToProfile] = useState(false);
  const navigate = useNavigate();

  // Get all users information and session data from the current logged-in user
  const [usersData, setUsersData] = useState([]);
  const [sessionData, setSessionData] = useState([]);
  const [pbFileName, setPbFileName] = useState("");

  // Persons attribute assignment React
  const [userId, setUserId] = useState("");
  const [firstname, setFirstname] = useState("");
  const [lastname, setLastname] =useState("");
  const [username, setUsername] = useState("");
  const [birthday, setBirthday] =useState("");
  const [biography, setBiography] =useState("");
  const [followedCount, setFollowedCount] = useState(0);
  const [followerCount, setFollowerCount] =useState(0);
  const [gender, setGender] = useState("");
  const [password, setPassword] = useState("");

  // Persons PostCount
  const [postCount, setPostCount] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Fetch users data
        const usersResponse = await fetch("https://localhost:7160/API/Users");
        const usersJsonData = await usersResponse.json();
        setUsersData(usersJsonData);

        // Fetch session data
        const sessionResponse = await fetch("https://localhost:7160/Api/UserDetails");
        const sessionJsonData = await sessionResponse.json();
        setSessionData(sessionJsonData);

        // Fetch PostCount data

        const postCountResponse = await fetch("https://localhost:7160/API/Users/Posts?id="+ {userId});
        const postCountJsonData = await postCountResponse.json();
        setPostCount(postCountJsonData.length);

        // Search the username of the Session User
        const currentUsername = sessionJsonData.Username;

        // Get the entire Person with all attributes and assign it to currentUser (object), where the Session User 
        // matches with all users.
        // (in short) Get the data from that specific person whos logged in
        const currentUser = usersJsonData.find(user => user.username === currentUsername);

        // If currentUser is truthy (i.e., not null, undefined, false, 0, or an empty string),
        // then the value of currentUser.userID is assigned to currentUserId.
        // If currentUser is falsy (i.e., null, undefined, false, 0, or an empty string), then an empty
        // string ("") is assigned to currentUserId.
        setUserId(currentUser ? currentUser.userID : "");
        setFirstname(currentUser ? currentUser.firstname : "");
        setLastname( currentUser ? currentUser.lastname :"");
        setUsername(currentUser ? currentUser.username : "" )
        setBirthday(currentUser ? currentUser.brithday: "") ;
        setBiography(currentUser ? currentUser.biography: "");
        setFollowedCount(currentUser ? currentUser.followedCount: 0);
        setFollowerCount(currentUser ? currentUser.followerCount: 0) ;
        setGender(currentUser ? currentUser.gender:"");

        // Check if the current user is in the list of all registered users and then retrieve the filePath from that API
        const currentUserData = usersJsonData.find(
          (user) => user.username === currentUsername
        );

        if (currentUserData) {
          setPbFileName(currentUserData.pbFileName);
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);


  const handleSubmit = (e) => {
    e.preventDefault(); // Prevent the default form submission behavior
    // Perform any necessary actions here, such as saving the form data or making API calls
    var formdata = new FormData();

    var requestOptions = {
      method: 'POST',
      body: formdata,
      redirect: 'follow'
    };
  
    fetch("https://localhost:7160/API/Users/Edit?userID="+userId+"&LastName=" + lastname+"&FirstName="+firstname +"&Username="+username+"&Password="+ password+"&Birthday="+birthday+ "&Biography="+ biography + "&Gender="+gender, requestOptions)
      .then(response => response.text())
      .then(result => console.log(result))
      .catch(error => console.log('error', error));

  };

  return (
    // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      <goToProfileContext.Provider value={{ goToProfile, setGoToProfile }}>
        <div className="beginning" id={theme}>
          <div className="sticky-header">
            <Header />
          </div>
          <div className="app">
            <Sidebar2 className ="sticky-sidebar" />
            <div className="Profile">
              <h1 style={{padding:'10px'}}>Profile: {username}</h1>
              <form  onClick={handleSubmit}>
                <div className="inputfields">
                  <h1 className="input_header">Name:</h1>
                  <div className="inputfiled_button">
                    <input
                      value={firstname}
                      onChange={(e) => setFirstname(e.target.value)}
                      placeholder="Firstname..."
                      type="text"
                      className="inputs"
                    />
                    <div className="line"></div> 
                  </div>
                  <h1 className="input_header">Lastname:</h1>
                  <input
                    value={lastname}
                    onChange={(e) => setLastname(e.target.value)}
                    placeholder="Lastname..."
                    type="text"
                    className="inputs"
                  />
                  <div className="line"></div> 
                  <h1 className="input_header">About:</h1>
                  <input
                    value={biography}
                    onChange={(e) => setBiography(e.target.value)}
                    placeholder="Tell something about you ..."
                    type="text"
                    className="inputs"
                  />
                  <div className="line"></div> 
                  <h1 className="input_header">Birthday: </h1>
                  <input
                    value={birthday}
                    onChange={(e) => setBirthday(e.target.value)}
                    placeholder="01.01.2000"
                    type="text"
                    className="inputs"
                  />
                  <div className="line"></div> 
                  <select
                    value={gender}
                    onChange={(e) => setGender(e.target.value)}
                    className="inputs"
                  >
                    <option value="">Select Gender</option>
                    <option value="male">Male</option>
                    <option value="female">Female</option>
                    <option value="Diverse">Diverse</option>
                  </select>
                  <div className="line"></div> 
                  <input
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    type="password"
                    placeholder="Enter new password"
                    className="inputs"
                  />
                  <div className="line"></div> 
                  <Button
                    className="zwitscherButton"
                    type="submit"
                  >
                    Save
                  </Button>
                </div>
              </form>
              <div className="statistics_profile">
                <h1>Followered by: </h1>
                <span>{followerCount}</span>
                <h1>Follows:</h1>
                <span>{followedCount}</span>
                <h1>Posts:</h1>
                <span>{postCount}</span>
              </div>
            </div>
          </div>
          <Routes>
            <Route path="/profile" element={<Profile />} />
          </Routes>
        </div>
      </goToProfileContext.Provider>
    </ThemeContext.Provider>
  );
}

export default Profile;
