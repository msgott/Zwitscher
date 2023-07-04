import React, { useState, useEffect } from "react";
import "./Feed.css";
import ZwitscherBox from "./ZwitscherBox";
import Post from "./Post";
{
  /*
import {db} from './firebase';
import { getDocs, collection } from "firebase/firestore";
*/
}

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

  {
    /*  With firebase - get the "table" posts from the database and put it in postsCollectionRef
  get all the posts from the database. -- setPosts transfers the data from filteredData into posts
  becasue of this dependecy: const [posts, setPosts] = useState([]);
  
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
  }, []);*/
  }

  return (
    <div className="feed">
      <div className="feed_header">{/*<h2>home</h2>*/}</div>
      <ZwitscherBox />

      {/*Firebase: 
        How to grap the "columns" and therefore the individual attributes from the database
        "table" and assign them to variables that will be given to the component post and post inherite the values
        
        {posts.map((post) => (
          <Post
          name = {post.name}
          text = {post.text}
          avatar = {post.avatar}
          image = {post.image}
          />
        ))}

        */}

      {/*Hardcoded posts -- data is given to the Post component -- open Post.js*/}
      <Post
        name="JD"
        text="My new Job - This text can be much longer"
        image="https://cdn.systematic.com/media/g0sj1tbg/hospital-building-001-global.jpg?cropAlias=hero_large&width=992&height=483&quality=80&rmode=crop&format=webp&null"
        avatar="data:image/jpeg;base64,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"
      />

      {postsData.map((post) => (
        <Post
          postId={post.postID}
          name={post.user_username}
          text={post.postText}
          image={"https://localhost:7160/Media/" + post.mediaList[0]}
          avatar={"https://localhost:7160/Media/" + post.user_profilePicture}
          rating={post.rating}
        />
      ))}

      <Post
        name="freeCodeCamp"
        text="Do you like programming? "
        image="https://media.istockphoto.com/id/1224500457/photo/programming-code-abstract-technology-background-of-software-developer-and-computer-script.jpg?s=612x612&w=0&k=20&c=nHMypkMTU1HUUW85Zt0Ff7MDbq17n0eVeXaoM9Knt4Q="
        avatar="data:image/png;base64,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"
      />

      <Post
        name="Russel Westbrook"
        text="It's a me.. Mario"
        image="https://static.giga.de/wp-content/uploads/2022/10/supermario-rcm850x478u.jpg"
        avatar="https://images.fineartamerica.com/images/artworkimages/mediumlarge/3/russell-westbrook-jennifer-pottheiser.jpg"
      />
    </div>
  );
}

export default Feed;
