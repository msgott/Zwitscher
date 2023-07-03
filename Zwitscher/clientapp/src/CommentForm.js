import React, { useState } from "react";

function CommentForm(autoFocus = false, onSubmit, initialValue = "") {
  const [message, setMessage] = useState(initialValue); // set values for the form/Default,Edit

  function handleSubmit(e) {
    e.preventDefault();
    onSubmit(message).then(
      () => setMessage("") //clears up the form
    );
  }

  return (
    <form onSubmit={handleSubmit}>
      <div class_name="comment-form-row">
        <textarea
          autoFocus={autoFocus}
          value={message}
          onChange={(m) => setMessage(m.target.value)}
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
