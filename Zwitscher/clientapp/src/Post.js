import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Comments from "./Comments";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import UpVote from "./Images/up-arrow-svgrepo-com.svg";
import DownVote from "./Images/down-arrow-svgrepo-com.svg";
import { goToProfileContext } from "./AppZwitscher";

{
  /*The hard coded stuff from the Feed components will be entered here as props/ name,text etc.*/
}
function Post({ postID, name, text, image, avatar }) {
  const { goToProfile, setGoToProfile } = useContext(goToProfileContext);

  {
    /*Handle Up- and Downvotes*/
  }
  const [votes, setVotes] = useState(0);
  const [setUp, hasSetUp] = useState(false);
  const [setDown, hasSetDown] = useState(false);

  const handleUpvoteClick = () => {
    if (setUp === false && setDown === false) {
      setVotes((count) => count + 1);
      hasSetUp(true);
    } else if (setUp === false && setDown === true) {
      setVotes((count) => count + 2);
      hasSetUp(true);
      hasSetDown(false);
    } else {
      setVotes((count) => count - 1);
      hasSetUp(false);
    }
  };

  const handleDownvoteClick = () => {
    if (setDown === false && setUp === false) {
      setVotes((count) => count - 1);
      hasSetDown(true);
    } else if (setDown === false && setUp === true) {
      setVotes((count) => count - 2);
      hasSetUp(false);
      hasSetDown(true);
    } else {
      setVotes((count) => count + 1);
      hasSetDown(false);
    }
  };

  {
    /*Open Comment section*/
  }
  const [showComments, setShowComments] = useState(false);

  const toggleComments = () => {
    setShowComments(!showComments);
  };

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
                onClick={handleUpvoteClick}
              />
              <span>{votes}</span>
              <img
                src={DownVote}
                alt="Icon"
                text="UpVote"
                className="downvote"
                onClick={handleDownvoteClick}
              />
            </div>
          </div>
          {showComments && <Comments />}
        </div>
      </div>
    </div>
  );
}

export default Post;
