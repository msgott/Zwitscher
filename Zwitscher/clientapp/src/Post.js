import React from 'react'
import './Post.css';
import Avatar from '@mui/material/Avatar';
import VerifiedIcon from '@mui/icons-material/Verified';
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import RepeatIcon from "@mui/icons-material/Repeat";
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import PublishIcon from "@mui/icons-material/Publish";


function Post({
    /*Likes and retweets possible functions etc.*/
    displayName,
    username,
    verified,
    text,
    image,
    avatar
}) {
  return (
    <div className="post">
        <div className="post_avatar">
        <Avatar src="https://www.tagesspiegel.de/politik/images/files-in-this-file-photo-taken-on-february-26-2022-former-us-president-donald-trump-speaks-at-the-conservative-political-action-conference-2022-cpac-in-orlando-florida-ex-president-donald-trump-and-his-eldest-children-are-scheduled-to-testify-in/alternates/BASE_21_9_W1000/files-in-this-file-photo-taken-on-february-26-2022-former-us-president-donald-trump-speaks-at-the-conservative-political-action-conference-2022-cpac-in-orlando-florida---ex-president-donald-trump-and-his-eldest-children-are-scheduled-to-testify-in-new-yorks-civil-probe-into-alleged-fraud-at.jpeg"/>
        </div>
        <div className="post_body">
            <div className="post_header">
                <div className="post_headerText">
                    <h3>
                        Donald Trump <span className="post_header_span">
                        <VerifiedIcon className="post_badge"></VerifiedIcon> {"@realDonaldTrump"}
                        </span>
                        
                    </h3>
                </div>
                <div className="post_headerDescription">
                    <p>Look what I found on the Internet</p>
                </div>
            </div>
            <img src="https://i.gifer.com/origin/d6/d66620ccdb4aee4182879a2c07d393ef_w200.gif" alt=""/>
        </div>
        <div className="post__footer">
            <ChatBubbleOutlineIcon fontSize="small" />
            <RepeatIcon fontSize="small" />
            <FavoriteBorderIcon fontSize="small" />
            <PublishIcon fontSize="small" />
        </div>
    </div>
    );
    };

export default Post;
