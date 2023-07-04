import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Comments from "./Comments";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import UpVote from "./Images/up-arrow-svgrepo-com.svg";
import DownVote from "./Images/down-arrow-svgrepo-com.svg";
import { goToProfileContext } from "./AppZwitscher";

//The hard coded stuff from the Feed components will be entered here as props/ name,text etc.
function Post({ postId, name, text, image, avatar, rating }) {
  const { goToProfile, setGoToProfile } = useContext(goToProfileContext);

  //Handle Up- and Downvotes
  const [votes, setVotes] = useState({ rating });
  const [setUp, hasSetUp] = useState(false);
  const [setDown, hasSetDown] = useState(false);

  const handleUpvoteClick = async (postId) => {
    if (!setUp && !setDown) {
      setVotes((prevVotes) => ({ rating: prevVotes.rating + 1 }));
      hasSetUp(true);
      await updateVoteOnBackend(postId, votes.rating + 1);
    } else if (!setUp && setDown) {
      setVotes((prevVotes) => ({ rating: prevVotes.rating + 2 }));
      hasSetUp(true);
      hasSetDown(false);
      await updateVoteOnBackend(postId, votes.rating + 2);
    } else {
      setVotes((prevVotes) => ({ rating: prevVotes.rating - 1 }));
      hasSetUp(false);
      await updateVoteOnBackend(postId, votes.rating - 1);
    }
  };

  const handleDownvoteClick = () => {
    if (!setDown && !setUp) {
      setVotes((prevVotes) => ({ rating: prevVotes.rating - 1 }));
      hasSetDown(true);
      updateVoteOnBackend(votes.rating - 1);
    } else if (!setDown && setUp) {
      setVotes((prevVotes) => ({ rating: prevVotes.rating - 2 }));
      hasSetUp(false);
      hasSetDown(true);
      updateVoteOnBackend(votes.rating - 2);
    } else {
      setVotes((prevVotes) => ({ rating: prevVotes.rating + 1 }));
      hasSetDown(false);
      updateVoteOnBackend(votes.rating + 1);
    }
  };

  //Send updated vote to backend
  const updateVoteOnBackend = async (postId, newRating) => {
    try {
      const response = await fetch(
        `https://localhost:7160/API/Posts/${postId}/Vote`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ rating: newRating }),
        }
      );
      // Handle the response if needed
    } catch (error) {
      console.error("Error updating vote:", error);
    }
  };

  //Open Comment section
  const [showComments, setShowComments] = useState(false);

  const toggleComments = () => {
    setShowComments(!showComments);
  };

  // Get authorization data from backend
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
    <div className="post">
      <div className="post_avatar">
        <Avatar src={avatar} />
        <div className="post_body">
          <div className="post_header">
            <div className="post_headerText">
              <h3>{name}</h3>
            </div>
            <div className="post_headerDescription"></div>
            <p>{text}</p>
            <p>{data.Username}</p>
          </div>
          <img src={image} alt="" />
          <div className="post_footer">
            <ChatBubbleOutlineIcon
              onClick={toggleComments}
              className="chat-icon"
            />
            <div className="vote_container">
              <img
                src={UpVote}
                alt="Icon"
                text="UpVote"
                className="upvote"
                onClick={() => handleUpvoteClick(postId)}
              />
              <span>{votes.rating}</span>
              <img
                src={DownVote}
                alt="Icon"
                text="UpVote"
                className="downvote"
                onClick={handleDownvoteClick}
              />
            </div>
          </div>
          {showComments && <Comments postId={postId} />}
        </div>
      </div>
    </div>
  );
}

export default Post;
