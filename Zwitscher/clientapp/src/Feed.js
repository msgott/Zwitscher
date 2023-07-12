import React, { useState, useEffect } from "react";
import "./Feed.css";
import ZwitscherBox from "./ZwitscherBox";
import Post from "./Post";
import { FaArrowCircleUp } from 'react-icons/fa';
import { Button } from "@mui/material";
function Feed({
    userid
}) {
    const [postsData, setPostsData] = useState([]);
    const [visible, setVisible] = useState(false)

    const toggleVisible = () => {
        const scrolled = document.getElementById("feed").scrollTop;


        if (scrolled > 300) {
            setVisible(true)
        }
        else if (scrolled <= 300) {
            setVisible(false)
        }
    };

    const scrollToTop = () => {
        document.getElementById("feed").scrollTo({
            top: 0,
            behavior: 'smooth'
            /* you can also use 'auto' behaviour
               in place of 'smooth' */
        });
    };
    if (document.getElementById("feed"))
        document.getElementById("feed").addEventListener('scroll', toggleVisible);
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

    //Get Posts information from backend
    useEffect(() => {
        const fetchPostsData = async () => {
            try {
                var url = window.location.href;
                var response = undefined;
                if (url.endsWith("/Zwitscher")) {


                    response = await fetch("https://localhost:7160/API/Posts"); // Replace with your API endpoint
                } else if (url.endsWith("/public")) {
                    response = await fetch("https://localhost:7160/API/OnlyPublicPosts"); // Replace with your API endpoint
                } else if (url.endsWith("/trending")) {
                    response = await fetch("https://localhost:7160/API/PostsSortedByRating"); // Replace with your API endpoint
                }else if (url.endsWith("/profile")) {
                    response = await fetch("https://localhost:7160/API/Users/Posts?id=" +userid? userid:sessionData.userID); // Replace with your API endpoint
                }

                console.log(response.text);
                const jsonData = await response.json();
                setPostsData(jsonData);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchPostsData();
    }, [sessionData]);



    // Get authorizationdata from backend
    const [data, setData] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch("https://localhost:7160/Api/UserDetails"); // Replace with your API endpoint
                const jsonData = await response.json();
                setData(jsonData);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();
    }, []);



    return (
        <div id="feed" className="feed" onScroll={() => { toggleVisible() }}>
            <div className="feed_header">{/*<h2>home</h2>*/}</div>
            {data.Username !== "" && <ZwitscherBox />}
            {postsData.length == 0 && (

                <div className="Loading">
                Loading feed Data...
                </div>
            )
            }
            {postsData.map((post) => (
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
                    commentCount={post.commentCount} />
            ))}
            <div className="scroll" onClick={scrollToTop} style={{ display: visible ? 'inline' : 'none' }}>
                Nach Oben
            </div>
        </div>

    );
}

export default Feed;
