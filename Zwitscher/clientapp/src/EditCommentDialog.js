import React, { useContext, useState, useEffect } from "react";
import "./EditCommentDialog.css";

import Avatar from "@mui/material/Avatar";

import { Button } from "@mui/material";




function EditCommentDialog({
    _commentId,
    _commentText,
    handleClose
}) {
    // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

    // set the theme to 'light mode' in the beginning and have the opportunity to change theme
    // depending on toggleTheme

    const [theme, setTheme] = useState("light");

    const toggleTheme = () => {
        setTheme((curr) => (curr === "light" ? "dark" : "light"));
    };
    // Navigate to the profile page if set to true. Follow goToProfileContext.Provider to understand
    // routing with React v18
    /*const navigate = useNavigate();*/

    // Get all users information and session data from the current logged-in user
    const [commentId, setCommentId] = useState("");
    const [commentText, setCommentText] = useState("");

    useEffect(() => {
        const setdefaultValues = async () => {
            try {

                setCommentId(_commentId);
                setCommentText(_commentText);


            } catch (error) {
                console.error("Error setting data:", error);
            }
        };

        setdefaultValues();
    }, []);



    async function editOwnComment(commentId, text) {
        if (!text) {
            alert("Bitte Kommentartext eingeben");
            return;
        }
        //console.log(text);
        try {
            const response = await fetch(
                "https://localhost:7160/API/Comments/Edit?id=" + commentId + "&CommentText=" + text,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            ).then((response) => response.text()).then((result) => console.log(result));
            handleClose();
            // Handle the response if needed
        } catch (error) {
            alert("Der Kommentar konnte nicht geändert werden");

        }
    };

    return (
        // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
        <div Class="form-container">

            <form id="commentform">

                <div >

                    <label> Text:</label>
                    <input type="text" id="CommentTextInput" placeholder="Kommentartext" onChange={(e) => { setCommentText(e.target.value) }} defaultValue={commentText}></input>

                    <Button
                        onClick={() => editOwnComment(commentId, commentText)}

                        
                    >
                        Speichern
                    </Button>
                </div>
            </form>
        </div>

    );
}

export default EditCommentDialog;