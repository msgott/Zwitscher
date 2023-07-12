import React, { useState, useEffect } from "react";
import "./Feed.css";
import ZwitscherBox from "./ZwitscherBox";
import Post from "./Post";
import { FaArrowCircleUp } from 'react-icons/fa';
import { Button } from "@mui/material";
function Feed() {
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
  //Get Posts information from backend
  useEffect(() => {
    const fetchPostsData = async () => {
      try {
        const response = await fetch("https://localhost:7160/API/Posts"); // Replace with your API endpoint
        const jsonData = await response.json();
        setPostsData(jsonData);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchPostsData();
  }, []);

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
      <div id="feed" className="feed" onScroll={() => { toggleVisible() } }>
          <div className="feed_header">{/*<h2>home</h2>*/}</div>
          {data.Username !== "" && <ZwitscherBox />}

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
