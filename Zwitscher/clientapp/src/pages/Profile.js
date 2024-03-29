﻿import { ChangeEvent, useState, createContext, useEffect } from "react";
import React, { useContext } from "react";
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';

import Modal from '@mui/material/Modal';
import {Routes,Route,useNavigate,useLocation,} from "react-router-dom";

import Header from "../Header";
import Sidebar2 from "../Sidebar2";

import "../Profile.css";
import { ThemeContext } from "../AppZwitscher";


import EditProfileDialog from "../EditProfileDialog";
import Feed from "../Feed";
import Post from "../Post";
import { useParams } from 'react-router-dom';

export const goToProfileContext = createContext(null);

const Profile = () => {

    const { state } = useLocation();
    const { profileUsername } = useParams();
    const navigate = useNavigate();
    if (profileUsername == undefined) {
        navigate('/Zwitscher')
    }
    const [userCounter, setuserCounter] = useState(0);
    //console.log(foreignUserObject);
    //Modal stuff------------------------------------------------
    const [EditProfileOpen, setEditProfileOpen] = React.useState(false);
    const EditProfilehandleOpen = () => setEditProfileOpen(true);
    const EditProfilehandleClose = () => setEditProfileOpen(false);




    //GET THEME 
    const location = useLocation();
    const screen = location.state?.screen;

    const [theme, setTheme] = useState(screen);
    //console.log("theme is: " +theme)

    const toggleTheme = () => {
      setTheme((curr) => (curr === "light" ? "dark" : "light"));
    };
    // Navigate to the profile page if set to true. Follow goToProfileContext.Provider to understand
    // routing with React v18

    const [goToProfile, setGoToProfile] = useState(false);
    /*const navigate = useNavigate();*/
    const [file, setFile] = useState(null);
    // Get all users information and session data from the current logged-in user

    const [ProfileCounter, setProfileCounter] = useState(0);
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
    //User Data
    const [userData, setUserData] = useState(null);
    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await fetch("https://localhost:7160/API/User?id=" + profileUsername);
                if (response.ok) {
                    const data = await response.json();
                    //console.log(data);
                    setUserData(data);
                } else {
                    console.log('Failed to fetch user data');
                }
            } catch (error) {
                console.log('Error fetching user data:', error);
            }
        };

        fetchUserData();
    }, [userCounter, ProfileCounter]);




    

    //FollowedBy
    const [userFollowedByData, setuserFollowedByData] = useState(null);

    useEffect(() => {
        const fetchUserFollowedBy = async () => {
            try {
                // Fetch session data
                var FollowedByResponse = await fetch("https://localhost:7160/API/Users/FollowedBy?UserID=" + profileUsername);
                var FollowedByJsonData = await FollowedByResponse.json();
                setuserFollowedByData(FollowedByJsonData);
                /*console.log(FollowedByJsonData);*/

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }
        fetchUserFollowedBy();

    }, []);


    //Follows
    const [userFollowingData, setuserFollowingData] = useState(null);

    useEffect(() => {
        const fetchUserFollows = async () => {
            try {
                // Fetch session data
                var FollowsResponse = await fetch("https://localhost:7160/API/Users/Following?UserID=" + profileUsername);
                var FollowsJsonData = await FollowsResponse.json();
                setuserFollowingData(FollowsJsonData);
                /*console.log(FollowsJsonData);*/

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }
        fetchUserFollows();

    }, []);

    //OwnFollows
    const [OwnuserFollowingData, setOwnuserFollowingData] = useState(null);

    useEffect(() => {
        const fetchUserOwnFollows = async () => {
            //console.log(sessionData);
            if (sessionData && sessionData.Username != "") {

                try {
                    // Fetch session data
                    var OwnFollowsResponse = await fetch("https://localhost:7160/API/Users/Following?UserID=" + sessionData.userID);
                    var OwnFollowsJsonData = await OwnFollowsResponse.json();
                    setOwnuserFollowingData(OwnFollowsJsonData);
                    //console.log(OwnFollowsJsonData);

                } catch (error) {
                    console.error("Error fetching data:", error);
                }
            }

        }
        fetchUserOwnFollows();
    }, [sessionData]);

    //OwnBlocks
    const [OwnuserBlockingData, setOwnuserBlockingData] = useState(null);

    useEffect(() => {
        const fetchUserOwnBlocking = async () => {
            //console.log(sessionData);
            if (sessionData && sessionData.Username!="") {

                try {
                    // Fetch session data
                    var OwnBlockingResponse = await fetch("https://localhost:7160/API/Users/Blocking?UserID=" + sessionData.userID);
                    var OwnBlockingJsonData = await OwnBlockingResponse.json();
                    setOwnuserBlockingData(OwnBlockingJsonData);
                    //console.log(OwnBlockingJsonData);

                } catch (error) {
                    console.error("Error fetching data:", error);
                }
            }

        }
        fetchUserOwnBlocking();
    }, [sessionData]);

    //Posts
    const [userPostData, setuserPostData] = useState(null);

    useEffect(() => {
        const fetchUserFollows = async () => {
            try {
                // Fetch session data
                var userPostDataResponse = await fetch("https://localhost:7160/API/Users/Posts?id=" + profileUsername);
                var userPostDataJsonData = await userPostDataResponse.json();
                setuserPostData(userPostDataJsonData);
                console.log(userPostDataJsonData);

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }
        fetchUserFollows();

    }, [ProfileCounter]);

    //Comments
    //const [userCommentData, setuserCommentData] = useState(null);

    //useEffect(() => {
    //    const fetchUserFollows = async () => {
    //        try {
    //            // Fetch session data
    //            var userCommentDataResponse = await fetch("https://localhost:7160/API/Users/Posts?id=" + foreignUserObject);
    //            var userCommentDataJsonData = await userCommentDataResponse.json();
    //            setuserCommentData(userCommentDataJsonData);
    //            //console.log(userCommentDataJsonData);

    //        } catch (error) {
    //            console.error("Error fetching data:", error);
    //        }
    //    }
    //    fetchUserFollows();

    //}, []);



    if (!userData || !sessionData) {
        return (
            <>
                <ThemeContext.Provider value={{ theme, toggleTheme }}>
                    <goToProfileContext.Provider value={{ goToProfile, setGoToProfile }}>
                        <div className="beginning" id={theme}>
                            <div className="sticky-header">
                                <Header />
                            </div>
                            <div className="app">
                                <Sidebar2 className="sticky-sidebar" />
                            </div>
                            <div className="Profile">
                                Loading...
                            </div>
                        </div>

                    </goToProfileContext.Provider>
                </ThemeContext.Provider>
            </>);
    }
    const { userID, lastname, firstname, username, birthday, biography, gender, followedCount, followerCount, pbFileName } = userData;



    var _pbfileName = ""
    if (!pbFileName) {

        _pbfileName = "real-placeholder.png";



    } else {

        _pbfileName = pbFileName;
    };

    /*const [pbFileName, setPbFileName] = useState(_pbfileName);*/


    const handleFileChange = (e) => {
        //sets file const based on selected file
        if (e.target.files) {
            setFile(e.target.files[0]);
        }
    };
    const isOwnProfile = () => {
        return sessionData.userID === userID;
        //checks if user is on own profile
    };

    const unfollow = async () => {
        //unfollows a user
        try {
            const response = await fetch(
                "https://localhost:7160/API/Users/Following/Remove?userToUnfollowId=" + profileUsername,

                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));
            window.location.reload(false);
            // Handle the response if needed
        } catch (error) {
            console.error("Error updating vote:", error);
        }
    };

    const follow = async () => {
        //follows a user
        try {
            const response = await fetch(
                "https://localhost:7160/API/Users/Following/Add?userToFollowId=" + profileUsername,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));
            window.location.reload(false);
            // Handle the response if needed
        } catch (error) {
            console.error("Error updating vote:", error);
        }
    };

    const unblock = async () => {
         //unblocks a user
        try {
            const response = await fetch(
                "https://localhost:7160/API/Users/Blocking/Remove?userToUnblockId=" + profileUsername,

                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));
            window.location.reload(false);
            // Handle the response if needed
        } catch (error) {
            console.error("Error updating vote:", error);
        }
    };

    const block = async () => {
        //blocks a user
        try {
            const response = await fetch(
                "https://localhost:7160/API/Users/Blocking/Add?userToBlockId=" + profileUsername,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));
            window.location.reload(false);
            // Handle the response if needed
        } catch (error) {
            console.error("Error updating vote:", error);
        }
    };
    /*console.log((OwnuserFollowingData && foreignUserObject !== sessionData.userID));*/

    return (
        // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
        <>
            <ThemeContext.Provider value={{ theme, toggleTheme }}>
                <goToProfileContext.Provider value={{ goToProfile, setGoToProfile }}>
                    <div className="beginning" id={theme}>
                        <div className="sticky-header">
                            <Header />
                        </div>
                        <div className="app">
                            <Sidebar2 className="sticky-sidebar" />
                            <div className="Profile">
                                <div className="profile_userbar">
                                    <img src={"/Media/" + (pbFileName !== "" ? pbFileName : "real-placeholder.png")} style={{ width: '300px', height: '300px' }}></img>
                                    <div>
                                    <h1>{username}</h1>
                                    <br/>
                                        <h5>{userData.biography}</h5>
                                    </div>
                                    <br></br>
                                    <div className="statistics_profile">
                                        <h4>Followers</h4>
                                        <span>{userData.followerCount}</span>
                                        <h4>Following</h4>
                                        <span>{userData.followedCount}</span>
                                        <h4>Posts</h4>
                                        <span>{userPostData !== null ? userPostData.length:0}</span>
                                        {/*<span>{postCount}</span>*/}

                                        {/*Profile Buttons*/}
                                        <div className = "Profile_buttons">
                                            {isOwnProfile() == true && (
                                                <Button className="EditProfileButton" onClick={EditProfilehandleOpen}>Bearbeiten</Button>
                                            )
                                            }
                                            {(OwnuserFollowingData && !isOwnProfile()) ? OwnuserFollowingData.find(user => user.userID === profileUsername) ?
                                                (<Button onClick={() => { unfollow() }}> Geflogt</Button>)
                                                :
                                                (<Button onClick={() => { follow() }}> Folgen</Button>)

                                                : ""}
                                            {(OwnuserBlockingData && !isOwnProfile()) ? OwnuserBlockingData.find(user => user.userID === profileUsername) ?
                                                (<Button onClick={() => { unblock() }}> Geblockt</Button>)
                                                :
                                                (<Button onClick={() => { block() }}> Blocken</Button>)
                                                : ""}
                                        </div>
                                    </div>

                                </div>


                                
                                <hr />
                                <div className="PostWrapper">
                                    {userPostData != null && userPostData.map((post) => (
                                        <Post
                                            key={post.postID}
                                            userId={post.userID}
                                            postId={post.postID}
                                            name={post.user_username}
                                            text={post.postText}
                                            image={post.mediaList}
                                            avatar={"https://localhost:7160/Media/" + post.user_profilePicture}
                                            rating={post.rating}
                                            _currentUserVoted={post.currentUserVoted}
                                            _userVoteIsUpvote={post.userVoteIsUpvote}
                                            _retweetsPost={post.retweetsPost}
                                            createdDate={post.createdDate}
                                            commentCount={post.commentCount}
                                            setFeedCounter={setProfileCounter}
                                        />
                                            
                                    ))}
                                    {(userPostData != null && userPostData.length == 0) && (

                                        <div>
                                            <span>
Noch keine Posts oder keine öffentlichen Posts!
                                            </span>
                                        </div>
                                    )}
                                </div>
                            </div>


                        </div>

                    </div>
                </goToProfileContext.Provider>
            </ThemeContext.Provider>

            <Modal
                open={EditProfileOpen}
                onClose={EditProfilehandleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"

            >


                <EditProfileDialog
                    userObject={userData}
                    handleClose={EditProfilehandleClose}
                    setuserCounter={setuserCounter}
                />


            </Modal>


        </>
    );
}

export default Profile;
