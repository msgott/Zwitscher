import React from 'react'
import './Widgets.css'
import SearchIcon from '@mui/icons-material/Search';
import {
  TwitterTimelineEmbed,
  TwitterShareButton,
  TwitterTweetEmbed,
} from "react-twitter-embed";

function Widgets() {
  return (
    <div>
        <div className="widgets">
          <div className="widgets_input">
            <SearchIcon className="widgets_searchIcon" />
              <input placeholder="Search Zwitscher" type="text" />
          </div>

          <div className="widgets_widgetContainer">
            <h2>What's happening</h2>
            <TwitterTweetEmbed tweetId={"1660736069258682369"} />
            <TwitterTimelineEmbed
              sourceType="profile"
              screenName="elonmusk"
              options={{height: 400}}
              />
          </div>
        </div>
    </div>
  )
}

export default Widgets