import React, { useState, useEffect } from "react";
import "./ReZwitscherBox.css";
import Button from "@mui/material/Button";
import Avatar from "@mui/material/Avatar";
import ImageIcon from "@mui/icons-material/Image";
import VideocamIcon from "@mui/icons-material/Videocam";

import PostPreview from "./PostPreview";
import Post from "./Post";

function ReZwitscherBox({
    postId,
    name,
    text,
    image,
    avatar,
    rating,
    _currentUserVoted,
    _userVoteIsUpvote,
    _handleClose,
    setFeedCounter
}
) {
    const [zwitscherMessage, setZwitscherMessage] = useState("");


    const sendZwitscher = async (e) => {
        e.preventDefault();
        if (zwitscherMessage.length == 0) {
            alert("Gebe bitte eine Textnachricht ein");
            return;
        }
        var requestOptions = {
            method: 'POST',
            redirect: 'follow'
        };

        var formdata = new FormData();
        formdata.append("TextContent", zwitscherMessage);
        formdata.append("IsPublic", true);
        formdata.append("retweetsID", postId);

       
        var requestOptions = {
            method: 'POST',
            body: formdata,
            redirect: 'follow'
        };
        try {
            fetch('https://localhost:7160/API/Posts/Add', requestOptions)
                .then((res) => res.json())
                .then((data) => console.log(data))
                .catch((err) => console.error(err));
            setZwitscherMessage("");
            _handleClose();
            setFeedCounter(Math.random);
        } catch (error){
            console.error(error);
        }
        
        

    };

    // Get all users information and session data from the current logged-in user
    const [usersData, setUsersData] = useState([]);
    const [sessionData, setSessionData] = useState([]);
    const [pbFileName, setPbFileName] = useState("");

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

                const currentUser = sessionJsonData.Username;

                // Check if the current user is in the list of all registered users and then retrieve the filePath from that API
                const currentUserData = usersJsonData.find(
                    (user) => user.username === currentUser
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

    

    
    
    return (
        <div className="zwitscherBox">
            <form>
                <div className="zwitscherBox_input">
                    <Avatar src={"/Media/"+pbFileName}></Avatar>
                    {/*Text Input*/}
                    <input
                        onChange={(e) => setZwitscherMessage(e.target.value)}
                        value={zwitscherMessage}
                        placeholder="Kommentiere den Zwitscher"
                        type="text"
                        maxLength="281"
                    />
                    
                </div>
                <PostPreview
                    postId={postId}
                    name={name}
                    text={text}
                    image={image}
                    avatar={avatar}
                    rating={rating}
                    _currentUserVoted={_currentUserVoted}
                    _userVoteIsUpvote={_userVoteIsUpvote}
                    isInRezwitscherBox={true}
                />
                <div className="zwitscherbox_footer">
                    
                    <div className="zwitscherbox_footerLeft">
                        
                        

                        
                    </div>
                    {/*Submit all Inputs*/}
                    <Button
                        onClick={sendZwitscher}
                        type="submit"
                        className="zwitscherBox_zwitscherButton"
                    >
                        ReZwitscher
                    </Button>
                </div>
            </form>
        </div>
    );
}

export default ReZwitscherBox;
