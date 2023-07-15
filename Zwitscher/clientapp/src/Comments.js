import React, { useState, useEffect } from "react";
import "./Comments.css";
import IconBtn from "./IconBtn.js";
import CommentForm from "./CommentForm";
import { FaEdit, FaHeart, FaRegHeart, FaReply, FaTrash } from "react-icons/fa";
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
import EditCommentDialog from "./EditCommentDialog";
import CommentCommentDialog from "./CommentCommentDialog";
import Comment from "./Comment"

const dateFormatter = new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
});

function Comments({ postId, sessionData, postusername}) {
    const [data, setData] = useState([]);
    const [commentToEdit, setcommentToEdit] = useState();
    const [commentToComment, setcommentToComment] = useState();
    const [commentToEditText, setcommentToEditText] = useState("");
    const [isReplying, setIsReplying] = useState(false);

    const [EditCommentopen, setEditCommentOpen] = React.useState(false);
    const EditCommenthandleOpen = () => setEditCommentOpen(true);
    const EditCommenthandleClose = () => setEditCommentOpen(false);
    const [CommentCommentopen, setCommentCommentOpen] = React.useState(false);
    const CommentCommenthandleOpen = () => setCommentCommentOpen(true);
    const CommentCommenthandleClose = () => setCommentCommentOpen(false);
    const [commentCounter, setCommentCounter] = useState(0);
    async function deleteOwnComment(postId, commentId) {
        //Deletes a Comment
        try
        {
            const response = await fetch(
                "https://localhost:7160/API/Posts/Comment/Remove?postId=" + postId + "&commentId=" + commentId,
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
            alert("Der Kommentar konnte nicht geloescht werden");

        }
    };
    function openCommentEditModal(commentId, text) {
        //Opens the Modal to edit a comment
        console.log("test");
        setcommentToEdit(commentId);
        setcommentToEditText(text)
        EditCommenthandleOpen();
        
    };

    function openCommentCommentModal(commentId) {
        //Opens the Modal to comment a comment
        console.log(commentId);
        setcommentToComment(commentId);
        
        CommentCommenthandleOpen();

    };
    

    /*Get data from the Database/API */

    useEffect(() => {
        const fetchData = async () => {
            //fetches all Comments of a Post
            try {
                const response = await fetch(
                    `https://localhost:7160/API/Posts/Comments?id=${postId}`
                ); // Replace with your API endpoint
                const jsonData = await response.json();
                setData(jsonData.length === 0 ? [] : jsonData);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
            //console.log("rerender comments first ")
        };
        
        fetchData();
    }, [commentCounter]);
    
    // Create Comment frontend logic CommentForm
    
    return (
        <>


            {/*Comment form with submit function*/}
            {(sessionData && sessionData.Username) !== "" && (
                <CommentForm postId={postId} setCommentCounter={setCommentCounter} />
            )}
            {/*Shows the comment section and renders it*/}
            <div className="commentWrapper">
                {data.map((comment) => (
                    <Comment
                        key={comment.commentId}
                        commentId={comment.commentId}
                        user_username={comment.user_username}
                        createdDate={comment.createdDate}
                        commentText={comment.commentText}
                        parentpostId={postId}
                        parentcommentId={""}
                        parentpostUsername={postusername}
                        parentcommentUsername={""}
                        sessionData={sessionData}
                        setCommentCounter= {setCommentCounter}
                    />
                    
                ))}
            </div>
            
        </>
    );
}

export default Comments;
