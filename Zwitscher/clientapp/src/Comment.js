import React, { useState, useEffect } from "react";
import "./Comments.css";
import IconBtn from "./IconBtn.js";
import CommentForm from "./CommentForm";
import { FaEdit, FaHeart, FaRegHeart, FaReply, FaTrash } from "react-icons/fa";
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
import EditCommentDialog from "./EditCommentDialog";
import CommentCommentDialog from "./CommentCommentDialog";
const dateFormatter = new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
});

function Comment({
    commentId,
    user_username,
    createdDate,
    commentText,
    parentpostId,
    parentpostUsername,
    parentcommentId,
    parentcommentUsername,
    sessionData,
    setCommentCounter
}) {
    
    const [data, setData] = useState([]);
    const [commentToEdit, setcommentToEdit] = useState();
    const [commentToComment, setcommentToComment] = useState();
    const [commentToEditText, setcommentToEditText] = useState("");
    const [isReplying, setIsReplying] = useState(false);

    const [EditCommentopen, setEditCommentOpen] = React.useState(false);
    const EditCommenthandleOpen = () => setEditCommentOpen(true);
    const EditCommenthandleClose = () => { setEditCommentOpen(false); setCommentCounter(Math.random);  }
    const [CommentCommentopen, setCommentCommentOpen] = React.useState(false);
    const CommentCommenthandleOpen = () => setCommentCommentOpen(true);
    const CommentCommenthandleClose = () => {
        setCommentCommentOpen(false);
        
        setCommentCounter(Math.random);
        
    }
    const [subCommentCounter, setsubCommentCounter] = useState(0);
    
    function openCommentEditModal(commentId, text) {
        //open the modal to edit a comment
        console.log("test");
        setcommentToEdit(commentId);
        setcommentToEditText(text)
        EditCommenthandleOpen();
        
    };

    function openCommentCommentModal(commentId) {
        //open the modal to comment a comment
        console.log(commentId);
        setcommentToComment(commentId);
        
        CommentCommenthandleOpen();

    };
    

    /*Get data from the Database/API */

    useEffect(() => {
        const fetchData = async () => {
            //fetches all Comments of a Comment
            try {
                const response = await fetch(
                    `https://localhost:7160/API/Comments/Comments?id=${commentId}`
                ); // Replace with your API endpoint
                const jsonData = await response.json();
                setData(jsonData.length === 0 ? [] : jsonData);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();
        console.log("fetchdata in Comment.js ausgeführt");
        //console.log(data);
    }, [subCommentCounter]);


    async function deleteOwnComment(postId, commentId) {
        //deletes a comment
        try {
            if (parentcommentId === "") {
                const response = await fetch(
                    "https://localhost:7160/API/Posts/Comment/Remove?postId=" + parentpostId + "&commentId=" + commentId,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify({

                        }),
                    }
                ).then((response) => response.text()).then((result) => console.log(result));
                setCommentCounter(Math.random);
            } else {
                const response = await fetch(
                    "https://localhost:7160/API/Comments/Comment/Remove?commentId=" + parentcommentId + "&commentToRemoveId=" + commentId,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify({

                        }),
                    }
                ).then((response) => response.text()).then((result) => console.log(result));
                setCommentCounter(Math.random);
            }
            // Handle the response if needed

        } catch (error) {
            alert("Der Kommentar konnte nicht gelöscht werden");

        }
    };
    // Create Comment frontend logic CommentForm

    return (
        <><div className="comment">
            <div className="comment-section">
                <div className="header" key={commentId}>
                    <span className="name">{user_username}</span>
                    <span className="date">{createdDate}</span>
                </div>
                <div className="message" key={commentId + "2"}>
                    <p>{commentText}</p>
                </div>
                <div className="footer">
                    {(sessionData && sessionData.Username) !== "" && (
                        <>
                            <IconBtn onClick={() => openCommentCommentModal(commentId)} Icon={FaReply} />
                            {user_username===sessionData.Username &&(
                            <IconBtn onClick={() => openCommentEditModal(commentId, commentText)} Icon={FaEdit} aria-label="Edit" />
                            )}
                            {(user_username === sessionData.Username || parentpostUsername === sessionData.Username || parentcommentUsername === sessionData.Username ) && (
                            <IconBtn onClick={() => deleteOwnComment(parentpostId, commentId)} Icon={FaTrash} aria-label="Delete" color="danger" />
                            )}
                            </>
                    )}
                    </div>
                <div className="childWrapper">
                    {data.map((comment) => (
                        <Comment
                        key = {comment.commentId}
                            commentId={comment.commentId}
                            user_username={comment.user_username}
                            createdDate={comment.createdDate}
                            commentText={comment.commentText}
                            parentpostId={""}
                            parentcommentId={commentId}
                            parentpostUsername={""}
                            parentcommentUsername={user_username}
                            sessionData={sessionData}
                            setCommentCounter={setsubCommentCounter}
                        />

                    ))}
                </div>
            </div>
            {isReplying && (
                <div className="mt-1 ml-3">
                    <CommentForm autoFocus onSubmit />
                </div>
            )}
        </div><Modal
            open={EditCommentopen}
            onClose={EditCommenthandleClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"

        >
                <EditCommentDialog
                    _commentId={commentToEdit}
                    _commentText={commentToEditText}
                    handleClose={EditCommenthandleClose} />


            </Modal>
            <Modal
                open={CommentCommentopen}
                onClose={CommentCommenthandleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"

            >
                <CommentCommentDialog
                    _commentId={commentToComment}
                    setsubCommentCounter={setsubCommentCounter }
                    handleClose={CommentCommenthandleClose} />


            </Modal></>
    );
}

export default Comment;
