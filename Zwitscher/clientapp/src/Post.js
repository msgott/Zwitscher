import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Comments from "./Comments";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import UpVote from "./Images/icons8-pfeil-50.png";
import DownVote from "./Images/icons8-pfeil-50-down_not_filled.png";
import VoteClicked from "./Images/icons8-arrow-50.png";
import VotedClickDown from "./Images/icons8-pfeil-50_down-filled.png";
import { goToProfileContext } from "./AppZwitscher";

//The hard coded stuff from the Feed components will be entered here as props/ name,text etc.
function Post({
  postId,
  name,
  text,
  image,
  avatar,
  rating,
  currentUserVoted,
  userVoteIsUpvote,
}) {
  const { goToProfile, setGoToProfile } = useContext(goToProfileContext);

  // Get the current filename for the Avatar
  const [userName, setUserName] = useState("");
  // Sessions Data from the current logged in User
  const [sessionData, setSessionData] = useState([]);
  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("https://localhost:7160/Api/UserDetails"); // Replace with your API endpoint
        const jsonData = await response.json();
        setSessionData(jsonData);
        // get the current Username
        const currentUser = jsonData.Username;

        const currentUserData = usersData.find(
          (user) => user.username === currentUser
        );

        if (currentUserData) {
          setUserName(currentUserData.pbFileName);
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);

  // Get all users information
  const [usersData, setUsersData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("https://localhost:7160/API/Users"); // Replace with your API endpoint
        const jsonData = await response.json();
        setUsersData(jsonData);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);

  // Get ratings and the variable for the ratings
  const [votes, setVotes] = useState({ rating });

  // Upvote
  const [CurrentUserVotedUp, setCurrentUserVotedUp] = useState("null");
  // Downvote
  const [CurrentUserVotedDown, setCurrentUserVotedDown] = useState(false);

  // ============================== upvote logic =============================================

  const handleUpvoteClick = async (postId, userVoteIsUpvote) => {
    // Current user has not voted at all, but votes up - increase by one
    if (CurrentUserVotedUp === "null" && CurrentUserVotedDown === false) {
      setVotes((votes) => ({ rating: votes.rating + 1 }));
      setCurrentUserVotedUp("true");
      await updateVoteOnBackend(postId, votes.rating + 1);
    }
    // Current user has not voted up, but voted already down - increases by 2
    else if (CurrentUserVotedUp === "false" && CurrentUserVotedDown === true) {
      setVotes((votes) => ({ rating: votes.rating + 2 }));
      setCurrentUserVotedUp("true");
      setCurrentUserVotedDown(false);
      await updateVoteOnBackend(postId, votes.rating + 2);
    }
    // Current user has voted up but clicks upvote again to neutralize the vote
    else if (CurrentUserVotedUp === "true" && CurrentUserVotedDown === false) {
      setVotes((votes) => ({ rating: votes.rating - 1 }));
      setCurrentUserVotedUp("null");
      await updateVoteOnBackend(postId, votes.rating - 1);
    }
  };

  // ============================== downvote logic =============================================
  // Neutral position
  const handleDownvoteClick = async (postId, userVoteIsUpvote) => {
    if (CurrentUserVotedUp === "null" && CurrentUserVotedDown === false) {
      setVotes((votes) => ({ rating: votes.rating - 1 }));
      setCurrentUserVotedUp("false");
      setCurrentUserVotedDown(true);
      await updateVoteOnBackend(votes.rating - 1);

      // Upvote was clicked before and now downvote is clicked
    } else if (
      CurrentUserVotedUp === "true" &&
      CurrentUserVotedDown === false
    ) {
      setVotes((votes) => ({ rating: votes.rating - 2 }));
      setCurrentUserVotedUp("false");
      setCurrentUserVotedDown(true);
      await updateVoteOnBackend(votes.rating - 2);
      // neutralize downvote
    } else {
      setVotes((votes) => ({ rating: votes.rating + 1 }));
      setCurrentUserVotedUp("null");
      setCurrentUserVotedDown(false);
      await updateVoteOnBackend(postId, votes.rating + 1);
    }
  };

  // Send votes to the backend depending on the posts ID and the user gets an update if the user
  // already voted for that post or not

  var requestOptions = {
    method: "POST",
    redirect: "follow",
  };

  fetch(
    "https://localhost:7160/API/Posts/Vote?postId=" +
      { postId } +
      "&IsUpVote=" +
      { userVoteIsUpvote } +
      ", requestOptions"
  )
    .then((response) => response.text())
    .then((result) => console.log(result))
    .catch((error) => console.log("error", error));

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
              {/*Upvote filled */}
              {CurrentUserVotedUp === "true" && (
                <img
                  src={VoteClicked}
                  alt="Icon"
                  text="ClickedIcon"
                  className="UpvoteFilled"
                  onClick={() => handleUpvoteClick(postId, userVoteIsUpvote)}
                />
              )}
              {/*Upvote NOT-filled */}
              {(CurrentUserVotedUp === "null" ||
                CurrentUserVotedUp === "false") && (
                <img
                  src={UpVote}
                  alt="Icon"
                  text="UpVote"
                  className="upvote"
                  onClick={() => handleUpvoteClick(postId, userVoteIsUpvote)}
                />
              )}
              <span>{votes.rating}</span>
              {/*Downvote NOT-filled */}
              {CurrentUserVotedDown === false && (
                <img
                  src={DownVote}
                  alt="Icon"
                  text="DownVote"
                  className="downvote"
                  onClick={() => handleDownvoteClick(postId, userVoteIsUpvote)}
                />
              )}
              {/*Downvote filled */}
              {CurrentUserVotedDown === true && (
                <img
                  src={VotedClickDown}
                  alt="Icon"
                  text="UpVote"
                  className="downvoteFilled"
                  onClick={() => handleDownvoteClick(postId, userVoteIsUpvote)}
                />
              )}
            </div>
          </div>
          {showComments && <Comments postId={postId} />}
        </div>
      </div>
    </div>
  );
}

export default Post;
