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
    
    async function deleteOwnComment(postId, commentId){
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
            alert("Der Kommentar konnte nicht gel�scht werden");

        }
    };
    function openCommentEditModal(commentId, text) {
        console.log("test");
        setcommentToEdit(commentId);
        setcommentToEditText(text)
        EditCommenthandleOpen();
        
    };

    function openCommentCommentModal(commentId) {
        console.log(commentId);
        setcommentToComment(commentId);
        
        CommentCommenthandleOpen();

    };
    

    /*Get data from the Database/API */

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(
                    `https://localhost:7160/API/Posts/Comments?id=${postId}`
                ); // Replace with your API endpoint
                const jsonData = await response.json();
                setData(jsonData.length === 0 ? [] : jsonData);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();
    }, [postId,openCommentEditModal, openCommentCommentModal]);
    
    // Create Comment frontend logic CommentForm

    return (
        <>


            {/*Comment form with submit function*/}
            {(sessionData && sessionData.Username) !== "" && (
                <CommentForm postId={postId} onSubmit />
            )}
            {/*Shows the comment section and renders it*/}
            <div className="commentWrapper">
                {data.map((comment) => (
                    <Comment
                        commentId={comment.commentId}
                        user_username={comment.user_username}
                        createdDate={comment.createdDate}
                        commentText={comment.commentText}
                        parentpostId={comment.postId}
                        parentcommentId={""}
                        parentpostUsername={postusername}
                        parentcommentUsername={""}
                        sessionData={sessionData}

                    />
                    
                ))}
            </div>
            
        </>
    );
}

export default Comments;
