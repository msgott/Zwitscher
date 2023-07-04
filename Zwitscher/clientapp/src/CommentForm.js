import React, { useState } from "react";

function CommentForm(postId, autoFocus = false, onSubmit, initialValue = "") {
  const [message, setMessage] = useState(initialValue); // set values for the form/Default,Edit

  // Function to submit the message (CommentText) including: Id,UserId,PostId, to the CommentController
  // to create a new comment and update the database
  async function handleSubmit(e) {
    e.preventDefault();

    try {
      const response = await fetch(
        "https://localhost:7160/API/Posts/Comment/Add",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            postId: postId,
            CommentText: message,
          }),
        }
      );

      if (response.ok) {
        // Comment added successfully
        console.log("Comment added successfully!");
        onSubmit();
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
    <form onSubmit={handleSubmit}>
      <div class_name="comment-form-row">
        <textarea
          autoFocus={autoFocus}
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          className="message-input"
        />
        <button className="btn" type="submit">
          Post
        </button>
      </div>
      {/*<div className="error-msg">{error}</div>*/}
    </form>
  );
}

export default CommentForm;
