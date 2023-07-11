import React, { useState, useEffect } from "react";
import "./Feed.css";
import ZwitscherBox from "./ZwitscherBox";
import Post from "./Post";

function Feed() {
  const [postsData, setPostsData] = useState([]);

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
    <div className="feed">
      <div className="feed_header">{/*<h2>home</h2>*/}</div>
      {data.Username !== "" && <ZwitscherBox />}

      {postsData.map((post) => (
        <Post
          postId={post.postID}
          name={post.user_username}
          text={post.postText}
          image={"https://localhost:7160/Media/" + post.mediaList[0]}
          avatar={"https://localhost:7160/Media/" + post.user_profilePicture}
          rating={post.rating}
          _currentUserVoted={post.currentUserVoted}
          _userVoteIsUpvote={post.userVoteIsUpvote}
        />
      ))}
    </div>
  );
}

export default Feed;
