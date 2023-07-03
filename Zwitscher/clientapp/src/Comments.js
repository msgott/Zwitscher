import React, { useState, useEffect } from "react";
import "./Comments.css";
import IconBtn from "./IconBtn.js";
import CommentForm from "./CommentForm";
import { FaEdit, FaHeart, FaRegHeart, FaReply, FaTrash } from "react-icons/fa";

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: "medium",
  timeStyle: "short",
});

function Comments({ postId, name }) {
  const [data, setData] = useState([]);

  {
    /*Get data from the Database/API */
  }
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
      <h1>Comments</h1>
      <CommentForm onSubmit />
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
              <IconBtn Icon={FaHeart} aria-label="Like">
                2
              </IconBtn>
              <IconBtn Icon={FaReply} aria-label="Reply" />
              <IconBtn Icon={FaEdit} aria-label="Edit" />
              <IconBtn Icon={FaTrash} aria-label="Delete" color="danger" />
            </div>
          </div>
        </div>
      ))}
    </>
  );
}

export default Comments;
