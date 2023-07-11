import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Comments from "./Comments";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import UpVote from "./Images/icons8-pfeil-50.png";
import DownVote from "./Images/icons8-pfeil-50-down_not_filled.png";
import VoteClicked from "./Images/icons8-arrow-50.png";
import RetzitscherIcon from "@mui/icons-material/Sync";
import VotedClickDown from "./Images/icons8-pfeil-50_down-filled.png";
import { goToProfileContext } from "./AppZwitscher";
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import ZwitscherBox from "./ZwitscherBox";
import ReZwitscherBox from "./ReZwitscherBox";

//The hard coded stuff from the Feed components will be entered here as props/ name,text etc.
function PostPreview({
    postId,
    name,
    text,
    image,
    avatar,
    rating,
    _currentUserVoted,
    _userVoteIsUpvote,
    isInRezwitscherBox
}) {
    const style = {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 600,
        bgcolor: 'background.paper',
        border: '2px solid #000',
        boxShadow: 24,
        p: 4,
    };
    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);



    const { goToProfile, setGoToProfile } = useContext(goToProfileContext);
    const [currentUserVoted, setcurrentUserVoted] = useState(_currentUserVoted);
    const [userVoteIsUpvote, setuserVoteIsUpvote] = useState(_userVoteIsUpvote === "true"? true : false);
    

    // Get the current filename for the Avatar
    const [userName, setUserName] = useState("");
    // Sessions Data from the current logged in User
    const [sessionData, setSessionData] = useState([]);


    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch("https://localhost:7160/Api/UserDetails"); // Replace with your API endpoint
                const jsonData = await response.json();
                //console.log(jsonData);
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



    // ============================== upvote logic =============================================

    const handleUpvoteClick = async (postId) => {
        // Current user has not voted at all, but votes up - increase by one
        if (!currentUserVoted) {
            setVotes((votes) => ({ rating: votes.rating + 1 }));

            await updateVoteOnBackend(postId, true);
            setcurrentUserVoted(true);
            setuserVoteIsUpvote(true);
        }
        // Current user has not voted up, but voted already down - increases by 2
        else if (currentUserVoted && !userVoteIsUpvote) {
            setVotes((votes) => ({ rating: votes.rating + 2 }));

            await updateVoteOnBackend(postId, true);
            setuserVoteIsUpvote(true);
        }
        // Current user has voted up but clicks upvote again to neutralize the vote
        else if (currentUserVoted && userVoteIsUpvote) {
            setVotes((votes) => ({ rating: votes.rating - 1 }));

            await updateVoteOnBackend(postId, true);
            setcurrentUserVoted(false);
            setuserVoteIsUpvote(null);
        }
    };

    // ============================== downvote logic =============================================
    // Neutral position
    const handleDownvoteClick = async (postId) => {
        if (!currentUserVoted) {
            setVotes((votes) => ({ rating: votes.rating - 1 }));

            await updateVoteOnBackend(postId, false);
            setcurrentUserVoted(true);
            setuserVoteIsUpvote(false);

            // Upvote was clicked before and now downvote is clicked
        } else if (
            currentUserVoted && userVoteIsUpvote
        ) {
            setVotes((votes) => ({ rating: votes.rating - 2 }));

            await updateVoteOnBackend(postId, false);

            setuserVoteIsUpvote(false);
            // neutralize downvote
        } else {
            setVotes((votes) => ({ rating: votes.rating + 1 }));

            await updateVoteOnBackend(postId, false);
            setcurrentUserVoted(false);
            setuserVoteIsUpvote(null);
        }
    };

    // Send votes to the backend depending on the posts ID and the user gets an update if the user
    // already voted for that post or not

    var requestOptions = {
        method: "POST",
        redirect: "follow",
    };

    //fetch(
    //  "https://localhost:7160/API/Posts/Vote?postId=" +
    //    { postId } +
    //    "&IsUpVote=" +
    //    { userVoteIsUpvote } +
    //    ", requestOptions"
    //)
    //  .then((response) => response.text())
    //  .then((result) => console.log(result))
    //  .catch((error) => console.log("error", error));

    //Send updated vote to backend
    const updateVoteOnBackend = async (postId, isupvote) => {
        try {
            const response = await fetch(
                `https://localhost:7160/API/Posts/Vote?postId=${postId}&IsUpVote=${isupvote}`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));

            // Handle the response if needed
        } catch (error) {
            console.error("Error updating vote:", error);
        }
    };
    //redirect to Login page
    const redirectToLogin = () => {
        window.location.replace("Auth");
    };

    //Open Comment section
    const [showComments, setShowComments] = useState(false);

    const toggleComments = () => {
        setShowComments(!showComments);
    };

    return (
        <><div className="post">
            <div className="post_avatar">
                <Avatar src={avatar} />
                <p>{name}</p>
            </div>
            <div className="post_body">
                <div className="post_header">
                    <div className="post_headerText">

                    </div>
                    <div className="post_headerDescription"></div>
                    <p>{text}</p>
                </div>
                {image.endsWith("mp4") && (
                    <video controls>
                        <source src={image} type="video/mp4" />
                        Your browser does not support the video tag.
                    </video>
                )}
                <img src={image} alt="" />
                {!isInRezwitscherBox && (
                <div className="post_footer">
                    
                    <><ChatBubbleOutlineIcon
                            onClick={toggleComments}
                            className="chat-icon" /><RetzitscherIcon
                                onClick={() => { handleOpen(); } }
                                className="chat-icon" /><div className="vote_container">


                                {(currentUserVoted && userVoteIsUpvote) ?
                                    /*Upvote filled */
                                    sessionData.Username === "" ? (<img
                                        src={VoteClicked}
                                        alt="Icon"
                                        text="ClickedIcon"
                                        className="UpvoteFilled"
                                        onClick={() => redirectToLogin()} />) : (<img
                                            src={VoteClicked}
                                            alt="Icon"
                                            text="ClickedIcon"
                                            className="UpvoteFilled"
                                            onClick={() => handleUpvoteClick(postId)} />)

                                    :
                                    /*Upvote NOT-filled */
                                    sessionData.Username === "" ? (<img
                                        src={UpVote}
                                        alt="Icon"
                                        text="UpVote"
                                        className="upvote"
                                        onClick={() => redirectToLogin()} />) : (<img
                                            src={UpVote}
                                            alt="Icon"
                                            text="UpVote"
                                            className="upvote"
                                            onClick={() => handleUpvoteClick(postId)} />)}

                                <span>{votes.rating}</span>

                                {(currentUserVoted && !userVoteIsUpvote) ?
                                    /*Downvote filled */
                                    !userVoteIsUpvote &&
                                        sessionData.Username === "" ? (<img
                                            src={VotedClickDown}
                                            alt="Icon"
                                            text="UpVote"
                                            className="downvoteFilled"
                                            onClick={() => redirectToLogin()} />) : (<img
                                                src={VotedClickDown}
                                                alt="Icon"
                                                text="UpVote"
                                                className="downvoteFilled"
                                                onClick={() => handleDownvoteClick(postId)} />)

                                    :
                                    /*Downvote NOT-filled */
                                    sessionData.Username === "" ? (<img
                                        src={DownVote}
                                        alt="Icon"
                                        text="DownVote"
                                        className="downvote"
                                        onClick={() => redirectToLogin()} />) : (<img
                                            src={DownVote}
                                            alt="Icon"
                                            text="DownVote"
                                            className="downvote"
                                            onClick={() => handleDownvoteClick(postId)} />)}
                            </div></>
                    
                    </div>
                )}
                {showComments && <Comments postId={postId} />}
            </div>
            
        </div><Modal
            open={open}
            onClose={handleClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
            >
        
                <Box sx={style}>
                    <ReZwitscherBox
                        postId={postId}
                        name={name}
                        text={text}
                        image={"https://localhost:7160/Media/" + image}
                        avatar={"https://localhost:7160/Media/" + avatar}
                        rating={rating}
                        _currentUserVoted={_currentUserVoted}
                        _userVoteIsUpvote={_userVoteIsUpvote}
                        _handleClose={handleClose}
                    />
                    
                </Box>
            </Modal></>
    );
}

export default PostPreview;


