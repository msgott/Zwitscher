import React, { useState } from "react";

function CommentForm({
  postId,
  autoFocus = false,
    setCommentCounter
  
}) {
  const [message, setMessage] = useState(""); // set values for the form/Default,Edit

  // Function to submit the message (CommentText) including: Id,UserId,PostId, to the CommentController
  // to create a new comment and update the database
  async function handleSubmit() {
    try {
      var requestOptions = {
        method: "POST",
        
        };
        if (message.length == 0) {
            alert('Bitte gebe einen Kommentartext ein')
            return;
        }
      let response = await fetch(
        "https://localhost:7160/API/Posts/Comment/Add?postId=" +
          postId +
          "&CommentText=" +
          message,
        requestOptions
      )
        

      if (response.ok) {
        // Comment added successfully
        console.log("Comment added successfully!");
          setCommentCounter(Math.random);
        // Clear the form after the submit
        setMessage("");
      } else {
        // Handle the error case
        console.error("Failed to add comment:", response.status);
      }
    } catch (error) {
      console.error("Error adding comment:", error);
    }
  }

  return (
    
          <div class_name="comment-form-row" >
              <textarea placeholder="Gebe einen Kommentar ein" 
          autoFocus={autoFocus}
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          className="message-input"
        />
              <button className="btn" onClick={() => { handleSubmit() }}  style={{ 'right': '0', 'left': 'auto' }}>
          Post
        </button>
      </div>
      
    
  );
}

export default CommentForm;
