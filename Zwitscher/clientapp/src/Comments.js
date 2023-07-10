import React, { useState, useEffect } from "react";
import "./Comments.css";
import IconBtn from "./IconBtn.js";
import CommentForm from "./CommentForm";
import { FaEdit, FaHeart, FaRegHeart, FaReply, FaTrash } from "react-icons/fa";
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
const dateFormatter = new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
});

function Comments({ postId, name }) {
    const [data, setData] = useState([]);
    const [commentToEdit, setcommentToEdit] = useState();
    const [commentToEditText, setcommentToEditText] = useState("");
    const [isReplying, setIsReplying] = useState(false);
    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
    const style = {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 400,
        height: 200,
        'overflow-y': 'auto',
        bgcolor: 'background.paper',
        border: '2px solid #000',
        'border-radius': '15px',
        boxShadow: 24,
        p: 4,
    };
    
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
            alert("Der Kommentar konnte nicht gelöscht werden");

        }
    };
    function openCommentEditModal(commentId, text) {
        console.log("test");
        setcommentToEdit(commentId);
        setcommentToEditText(text)
        handleOpen();
        
    };
    async function editOwnComment(commentId, text) {
        try {
            const response = await fetch(
                "https://localhost:7160/API/Comments/Edit?id="+commentId+"&CommentText=" + text,
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
            alert("Der Kommentar konnte nicht geändert werden");

        }
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
    }, [postId]);
    
    // Create Comment frontend logic CommentForm

    return (
        <>


            {/*Comment form with submit function*/}
            <CommentForm postId={postId} onSubmit />

            {/*Shows the comment section and renders it*/}
            <div className="commentWrapper">
                {data.map((comment) => (
                    <div className="comment">
                        <div className="comment-section">
                            <div className="header" key={comment.commentId}>
                                <span className="name">{comment.user_username}</span>
                                <span className="date">{comment.createdDate}</span>
                            </div>
                            <div className="message" key={comment.commendId}>
                                <p>{comment.commentText}</p>
                            </div>
                            <div className="footer">
                                
                                <IconBtn onClick={() => setIsReplying(prev => !prev)} isActive={isReplying} Icon={FaReply} />
                                <IconBtn onClick={() => openCommentEditModal(comment.commentId, comment.commentText)} Icon={FaEdit} aria-label="Edit" />
                                <IconBtn onClick={() => deleteOwnComment(postId, comment.commentId) } Icon={FaTrash} aria-label="Delete" color="danger" />
                            </div>
                        </div>
                        {isReplying && (
                            <div className="mt-1 ml-3">
                                <CommentForm autoFocus onSubmit />
                            </div>
                        )}
                    </div>
                ))}
            </div>
            <Modal
                open={open}
                onClose={handleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"
                
            >

                <Box sx={style}>
                    <label> Text:</label>
                    <input type="text" id="CommentTextInput" onChange={(e) => {setcommentToEditText(e.target.value) } } defaultValue={commentToEditText}></input>
                    <br/>
                    <button onClick={() => editOwnComment(commentToEdit,commentToEditText) }>Anpassen</button>
                </Box>
            </Modal>
        </>
    );
}

export default Comments;
