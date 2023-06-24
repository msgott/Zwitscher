import React, { useContext } from "react";
import "./Post.css";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import RepeatIcon from "@mui/icons-material/Repeat";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import PublishIcon from "@mui/icons-material/Publish";
import { goToProfileContext } from "./AppZwitscher";

{
  /*The hard coded stuff from the Feed components will be entered here as props/ name,text etc.*/
}
function Post({ postID, name, text, image, avatar }) {
  const { goToProfile, setGoToProfile } = useContext(goToProfileContext);

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
            </div>
            <div className="post_headerDescription">
              <p>{text}</p>
            </div>
          </div>
          <img src={image} alt="" />
          <div className="post_footer">
            <ChatBubbleOutlineIcon
              onClick={handleClick}
              className="chat-icon"
            />
            <RepeatIcon className="repeat-icon" />
            <FavoriteBorderIcon className="favorite-icon" />
            <PublishIcon className="publish-icon" />
          </div>
        </div>
      </div>
    </div>
  );
}

export default Post;
