import React, { useState, useEffect} from "react";
import './Feed.css';
import ZwitscherBox from './ZwitscherBox';
import Post from './Post';
import {db} from './firebase';
import { getDocs, collection } from "firebase/firestore";




function Feed() {
  const [posts, setPosts] = useState([]);

  const postsCollectionRef = collection(db , "posts");

  useEffect(()=> {
    const getPostsList = async () => {
      try {
        const data = await getDocs(postsCollectionRef);
        const filteredData = data.docs.map((doc) => ({...doc.data(),id: doc.id,}));
        setPosts(filteredData);
      } catch(err){
        console.error(err);
      }
      
    };

    getPostsList();
  }, []);

  return (
    <div className="feed">
        <div className="feed_header">
            {/*<h2>home</h2>*/}
        </div>
        <ZwitscherBox />

        {posts.map((post) => (
          <Post
          name = {post.name}
          text = {post.text}
          avatar = {post.avatar}
          image = {post.image}
          />
        ))}
        
    </div>
  )
}

export default Feed
