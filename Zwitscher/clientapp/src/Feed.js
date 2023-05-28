import React, { useState} from "react";
import './Feed.css';
import ZwitscherBox from './ZwitscherBox';
import Post from './Post';
/*import db from "./firebase";

import FlipMove from "react-flip-move";*/




function Feed() {
  const [posts, setPosts] = useState([]);



  return (
    <div className="feed">
        <div className="feed_header">
            <h2>home</h2>
        </div>
        <ZwitscherBox />
        <Post />
        <Post />
        <Post />
        <Post />
        <Post />
        <Post />
    </div>
  )
}

export default Feed
