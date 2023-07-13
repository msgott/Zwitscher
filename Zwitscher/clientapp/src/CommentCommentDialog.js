import React, { useContext, useState, useEffect } from "react";
import "./CommentCommentDialog.css";

import Avatar from "@mui/material/Avatar";

import { Button } from "@mui/material";




function CommentCommentDialog({
    _commentId,
    
    handleClose
}) {
    // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

    // set the theme to 'light mode' in the beginning and have the opportunity to change theme
    // depending on toggleTheme
    //console.log("asda" + _commentId);
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
                


            } catch (error) {
                console.error("Error setting data:", error);
            }
        };

        setdefaultValues();
    }, []);



    async function CommentComment(commentId, text) {
        if (!text) {
            alert("Bitte Kommentartext eingeben");
            return;
        }
        /*console.log(commentId);*/
        try {
            var response = await fetch(
                "https://localhost:7160/API/Comments/Comment/Add?commentId=" + commentId + "&CommentText=" + text,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({

                    }),
                }
            );
            handleClose();
            //alert(response.text());
            // Handle the response if needed
        } catch (error) {
            alert("Der Kommentar konnte nicht gesendet werden");

        }
    };

    return (
        // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
        <div Class="form-container">

            <form id="commentform">

                <div >

                    <label> Kommentieren:</label>
                    <input type="text" id="CommentTextInput" placeholder="Kommentiere den Kommentar" onChange={(e) => { setCommentText(e.target.value) }} defaultValue={commentText}></input>

                    <Button
                        onClick={() => CommentComment(commentId, commentText)}

                        
                    >
                        senden
                    </Button>
                </div>
            </form>
        </div>

    );
}

export default CommentCommentDialog;