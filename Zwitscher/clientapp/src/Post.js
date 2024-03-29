import React, { useContext, useState, useEffect } from "react";
import "./Post.css";
import Comments from "./Comments";
import Avatar from "@mui/material/Avatar";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import RetzitscherIcon from "@mui/icons-material/Sync";
import UpVote from "./Images/icons8-pfeil-50.png";
import DownVote from "./Images/icons8-pfeil-50-down_not_filled.png";
import VoteClicked from "./Images/icons8-arrow-50.png";
import VotedClickDown from "./Images/icons8-pfeil-50_down-filled.png";
import { goToProfileContext } from "./AppZwitscher";
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import ZwitscherBox from "./ZwitscherBox";
import ReZwitscherBox from "./ReZwitscherBox";

import PostPreview from "./PostPreview";
import { Route, useNavigate } from "react-router-dom";
import Profile from "./pages/Profile";
import Carousel from 'react-multi-carousel';
import 'react-multi-carousel/lib/styles.css';
import EditPostDialog from "./EditPostDialog";


//The hard coded stuff from the Feed components will be entered here as props/ name,text etc.
function Post({
    userId,
    postId,
    name,
    text,
    image,
    avatar,
    rating,
    createdDate,
    commentCount,
    _currentUserVoted,
    _userVoteIsUpvote,
    _retweetsPost,
    theme,
    setFeedCounter
    
}) {

    const [retweetsData, setRetweetsData] = useState();

    //Get Posts information from backend
    useEffect(() => {
        const fetchRetweetsData = async () => {
            try {
                if (_retweetsPost !== "") {
                    //console.log("RETWEETS: "+_retweetsPost);
                    const response = await fetch("https://localhost:7160/API/Post?id=" + _retweetsPost); // Replace with your API endpoint
                    const jsonData = await response.json();
                    setRetweetsData(jsonData);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchRetweetsData();
    }, []);

    const style = {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 600,
        height: 600,
        'overflow-y': 'auto',
        bgcolor: 'background.paper',
        border: '2px solid #000',
        'border-radius': '15px',
        boxShadow: 24,
        p: 4,
    };
    const style2 = {
        flex: '1'

    };
    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
    const [EditPostopen, setEditPostopen] = React.useState(false);
    const EditPosthandleOpen = (id, text, _public, retweets) => {
        //Opens the Edit modal
        setpostToEditId(id);
        setpostToEditText(text);
        setpostToEditPublic(_public);
        setpostToEditRetzwitscher(retweets);
        setEditPostopen(true);
    };
    const EditPosthandleClose = () => setEditPostopen(false);
    const [postToEditId, setpostToEditId] = useState("");
    const [postToEditText, setpostToEditText] = useState("");
    const [postToEditPublic, setpostToEditPublic] = useState(true);
    const [postToEditRetzwitscher, setpostToEditRetzwitscher] = useState("");
    


    const [currentUserVoted, setcurrentUserVoted] = useState(_currentUserVoted);
    const [userVoteIsUpvote, setuserVoteIsUpvote] = useState(_userVoteIsUpvote === "true" ? true : false);

    const [currentSlide, setCurrentSlide] = useState(0);
    // Get the current filename for the Avatar
    const [userName, setUserName] = useState("");
    // Sessions Data from the current logged in User
    const [sessionData, setSessionData] = useState([]);


    useEffect(() => {
        const fetchData = async () => {
            //gets the session Data in context of current user session
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
            //gets all Users
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

    
    const deletePost = async (postId) => {
        //deletes a Post
        try {
            const response = await fetch(
                `https://localhost:7160/API/Posts/Remove?id=${postId}`,
                {
                    method: "DELETE",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));

            setFeedCounter(Math.random);
            if (window.location.href.includes("/profile/")) { window.location.reload(); }
            // Handle the response if needed
        } catch (error) {
            console.error("Error deleting post:", error);
        }
    };
    //Send updated vote to backend
    const updateVoteOnBackend = async (postId, isupvote) => {
        //Update Votes for Post in backend
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
        window.location.href = "https://localhost:7160/Auth";
    };

    //Open Comment section
    const [showComments, setShowComments] = useState(false);

    const toggleComments = () => {
        setShowComments(!showComments);
    };
    const responsive = {
        desktop: {
            breakpoint: { max: 3000, min: 1024 },
            items: 3,
        },
        tablet: {
            breakpoint: { max: 1024, min: 464 },
            items: 1,
        },
        mobile: {
            breakpoint: { max: 464, min: 0 },
            items: 1,
        },
    };
    const handleSlideChange = (current) => {
        //setCurrentSlide(current);
    };
    const navigate = useNavigate();
    return (
        <><div className="post">
            <div className="post_avatar">
                {postId !== "00000000-0000-0000-0000-000000000000" ? (
                    <><Avatar onClick={() => { navigate(`/profile/${userId}`, { state: { screen: theme.value } }); }} src={avatar} /><p onClick={() => { navigate(`/profile/${userId}`, { state: { screen: theme.value } }); }}>{name}</p></>
                ) :
                    (
                        <><Avatar src={avatar} /><p>{name}</p></>

                    )}
                <h5 >{createdDate}</h5>
                {sessionData.Username === name &&(
                <><DeleteIcon
                        onClick={() => { deletePost(postId); } }
                        className="chat-icon" />
                        <EditIcon
                            onClick={() => { EditPosthandleOpen(postId, text, true, _retweetsPost) }}
                            className="chat-icon" 
                        />
                    </>       
                )}
            </div>
            <div className="post_body">
                <div className="post_header">
                    <div className="post_headerText">

                    </div>
                    <div className="post_headerDescription"></div>
                    <p>{text}</p>
                </div>

                {(_retweetsPost !== "" && retweetsData) ? (<PostPreview
                    postId={retweetsData.postID}
                    name={retweetsData.user_username}
                    text={retweetsData.postText}
                    image={retweetsData.mediaList}
                    avatar={"https://localhost:7160/Media/" + retweetsData.user_profilePicture}
                    rating={retweetsData.rating}
                    _currentUserVoted={retweetsData.currentUserVoted}
                    _userVoteIsUpvote={retweetsData.userVoteIsUpvote}
                    isInRezwitscherBox={false}

                />) : (

                    <>

                        <div className="carousel-container">
                            <Carousel
                                responsive={responsive}
                                beforeChange={handleSlideChange}
                                containerClass="carousel">
                                {image.map((image, index) => (
                                    <div key={`image-${index}`} className="carousel-item">
                                        {image.endsWith("mp4") ? (
                                            <video controls className="carousel-video">
                                                <source src={"https://localhost:7160/Media/" + image} type="video/mp4" />
                                                Your browser does not support the video tag.
                                            </video>) :
                                            <img src={"https://localhost:7160/Media/" + image} alt={`Image ${index}`} />
                                        }

                                    </div>
                                ))}

                            </Carousel>
                        </div>

                    </>



                )}
                {postId !== "00000000-0000-0000-0000-000000000000" && (
                    <div className="post_footer">
                        <ChatBubbleOutlineIcon
                            onClick={toggleComments}
                            className="chat-icon" > <p>{commentCount}</p></ChatBubbleOutlineIcon>
                        {sessionData.Username === "" ? (
                            <RetzitscherIcon
                                onClick={() => redirectToLogin()}
                                className="chat-icon"
                            />) :
                            (<RetzitscherIcon
                                onClick={handleOpen}
                                className="chat-icon"
                            />)}

                        <div className="vote_container">


                            {(currentUserVoted && userVoteIsUpvote) ?
                                /*Upvote filled */
                                sessionData.Username === "" ?
                                    (
                                        <img
                                            src={UpVote}
                                            alt="Icon"
                                            text="ClickedIcon"
                                            className="upvote"
                                            onClick={() => redirectToLogin()}
                                        />
                                    )
                                    :
                                    (
                                        <img
                                            src={VoteClicked}
                                            alt="Icon"
                                            text="ClickedIcon"
                                            className="UpvoteFilled"
                                            onClick={() => handleUpvoteClick(postId)}
                                        />
                                    )

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
                                        src={DownVote}
                                        alt="Icon"
                                        text="UpVote"
                                        className="downvote"
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
                        </div>
                        

                    </div>

                )}
            </div>
            {showComments && <Comments postId={postId} sessionData={sessionData} postusername={userName} />}
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
                        image={image}
                        avatar={avatar}
                        rating={rating}
                        _currentUserVoted={_currentUserVoted}
                        _userVoteIsUpvote={_userVoteIsUpvote}
                        _handleClose={handleClose}
                        setFeedCounter={(e) => { setFeedCounter(e) }}
                    />

                </Box>
            </Modal>
            <Modal
                open={EditPostopen}
                onClose={EditPosthandleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"
            >

                <Box sx={style}>
                    <EditPostDialog
                        postId={postToEditId}
                        postText={postToEditText}
                        postPublic={postToEditPublic}
                        postRetzwitscher={postToEditRetzwitscher}
                        handleClose={EditPosthandleClose}
                        setFeedCounter={(e) => { setFeedCounter(e) }}
                    />

                </Box>
            </Modal>

        </>
    );
}

export default Post;


