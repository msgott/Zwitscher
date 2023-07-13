import { Avatar, Button } from "@mui/material";
import React, { useEffect, useState } from "react";
import "./Widgets.css";

import SearchIcon from "@mui/icons-material/Search";
import { useNavigate } from "react-router-dom";

function Widgets() {
    //User Data
    const [searchText, setsearchText] = useState("");
    const [userData, setUserData] = useState([]);
    useEffect(() => {
        const fetchUserData = async () => {
            try {
                var response;
                if (searchText === "") {
                    response   = await fetch("https://localhost:7160/API/Users");
                } else {
                    response = await fetch("https://localhost:7160/API/Users/Search?searchString="+searchText); 
                }
                if (response.ok) {
                    const data = await response.json();
                    /*console.log(data);*/
                    setUserData(data);
                } else {
                    console.log('Failed to fetch user data');
                }
            } catch (error) {
                console.log('Error fetching user data:', error);
            }
        };

        fetchUserData();
    }, [searchText]);


    const searchUser = async (e) => {
        e.preventDefault();
        
        //fetch('https://localhost:7160/API/Posts/Add', requestOptions)
        //    .then((res) => res.json())
        //    .then((data) => console.log(data))
        //    .catch((err) => console.error(err));
        //setZwitscherMessage("");
        //setFiles(Array.from([]));
        //document.getElementById("fileselect").value = null;

    };

    const handleSearchChange =  (text) => {
        setsearchText(text);

        //fetch('https://localhost:7160/API/Posts/Add', requestOptions)
        //    .then((res) => res.json())
        //    .then((data) => console.log(data))
        //    .catch((err) => console.error(err));
        //setZwitscherMessage("");
        //setFiles(Array.from([]));
        //document.getElementById("fileselect").value = null;

    };
    //Session Data
    const [sessionData, setSessionData] = useState(null);

    useEffect(() => {
        const fetchUserSession = async () => {
            try {
                // Fetch session data
                var sessionResponse = await fetch("https://localhost:7160/Api/UserDetails");
                var sessionJsonData = await sessionResponse.json();
                setSessionData(sessionJsonData);


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }
        fetchUserSession();

    }, []);


    const navigate = useNavigate();

    return (

        <div className="widgets">
            {(sessionData && sessionData.Username) !== "" ? (
            <><div className="widgets_input">

                    <input placeholder="Suche Nutzer" type="text" onChange={(e) => { handleSearchChange(e.target.value); } } />

                </div><div className="searchResults">
                        <ul>
                            {userData.length == 0 && (
                                <li key={1}>
                                    <div>
                                        <div className="post_avatar">

                                            <Avatar src={"https://localhost:7160/Media/zwitscherlogo.png"} /><p>{"Keine Nutzer gefunden :("}</p>






                                        </div>
                                    </div>
                                </li>

                            )}
                            {userData.map((user) => (
                                <li key={user.userID}>
                                    <div>
                                        <div className="post_avatar">

                                            <Avatar onClick={() => { navigate(`/profile/${user.userID}`); } } src={"https://localhost:7160/Media/" + user.pbFileName} /><p onClick={() => { navigate(`/profile/${user.userID}`); } }>{user.username}</p>






                                        </div>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    </div></>
            ) :
                <div className= "Notification_widget">
                    <span>
                        Angemeldete Nutzer haben mehr Funktionen!
                    </span>
                    <Button onClick={() => { window.location.href = "https://localhost:7160/Auth"; } }>Anmelden</Button>
                </div>
        
        }
        </div>
    
    );
}

export default Widgets;
