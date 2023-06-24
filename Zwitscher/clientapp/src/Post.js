import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import NorthIcon from "@mui/icons-material/North";
import SouthIcon from "@mui/icons-material/South";
import ThumbUpOffAltIcon from "@mui/icons-material/ThumbUpOffAlt";
import ThumbDownOffAltIcon from "@mui/icons-material/ThumbDownOffAlt";
import { goToProfileContext } from "./AppZwitscher";

{
  /*The hard coded stuff from the Feed components will be entered here as props/ name,text etc.*/
}
function Post({ postID, name, text, image, avatar }) {
  const { goToProfile, setGoToProfile } = useContext(goToProfileContext);
  const [upvotes, setUpvotes] = useState(0);
  const [downvotes, setDownvotes] = useState(0);

  const handleUpvoteClick = () => {
    setUpvotes((count) => count + 1);
  };

  const handleDownvoteClick = () => {
    setDownvotes((count) => count + 1);
  };

  {
    /*Function needs to be replaced later but maybe beneficial to understand routing in react?*/
  }
  const handleClick = () => {
    setGoToProfile(true);
  };

  return (
    <div className="post">
      <div className="post_avatar">
        <Avatar src={avatar} />
        <div className="post_body">
          <div className="post_header">
            <div className="post_headerText">
              <h3>{name}</h3>
              <div className="post_votes">
                <div className="post_upVotes">
                  <ThumbUpOffAltIcon className="post_badge" />
                  <span className="post_header_span">{upvotes}</span>
                </div>
                <div className="post_downVotes">
                  <ThumbDownOffAltIcon className="post_badge" />
                  <span className="post_header_span">{downvotes}</span>
                </div>
              </div>
            </div>
            <div className="post_headerDescription"></div>
          </div>
          <img src={image} alt="" />
          <div className="post_footer">
            <ChatBubbleOutlineIcon
              onClick={handleClick}
              className="chat-icon"
            />
            <NorthIcon className="upvote-icon" onClick={handleUpvoteClick} />
            <SouthIcon
              className="downvote-icon"
              onClick={handleDownvoteClick}
            />
          </div>
        </div>
      </div>
    </div>
  );
}

export default Post;
