import React, { useContext, useState, useEffect } from "react";
import "./EditPostDialog.css";

import Avatar from "@mui/material/Avatar";

import { Button, Modal } from "@mui/material";
import { useNavigate } from "react-router-dom";




function EditPostDialog({
    postId,
    postText,
    postPublic,
    postRetzwitscher,
    handleClose,
    setFeedCounter
}) {
    


    // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

    // set the theme to 'light mode' in the beginning and have the opportunity to change theme
    // depending on toggleTheme
    
    const [theme, setTheme] = useState("light");

    const toggleTheme = () => {
        setTheme((curr) => (curr === "light" ? "dark" : "light"));
    };


    
    
    
    

    // Persons attribute assignment React
    
   


    

    const [Text, setText] = useState(postText);
    const [Public, setPublic] = useState(postPublic);
    const [Retzwitscher, setRetzwitscher] = useState(postRetzwitscher!=="");
    const handleFileChange = (e) => {
       
    };
    const handleSubmit = (e) => {
        e.preventDefault();

        if(Text === "") alert("Bitte gebe einen Text ein.")
        console.log(postRetzwitscher);
        var retweettemp = "";
        if (Retzwitscher) {
            retweettemp = postRetzwitscher;
        }


        var formdata = new FormData();
        formdata.append("postID", postId);
        formdata.append("TextContent", Text);
        formdata.append("IsPublic", Public);
        formdata.append("retweetsID", retweettemp);

        var requestOptions = {
            method: 'POST',
            body: formdata,
            redirect: 'follow'
        };

        var response = fetch("https://localhost:7160/API/Posts/Edit", requestOptions);
            

        if (response.ok) {
            
            handleClose();
            setFeedCounter(Math.random);
        } else {
            window.location.reload();
        }
        
        if (window.location.href.includes("/profile/")) { window.location.reload(); }
    };
    const navigate = useNavigate();
    
    

    return (
        // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
        <>
            
            <div Class="form-container">
                
                


               
                    
                    <div>
                       
                        <label >Posttext:</label>
                        <input
                            id="Post-text"
                            defaultValue={postText}
                            onChange={(e) => setText(e.target.value)}
                            placeholder="Text..."
                            type="text"

                            />
                    <label >Post ist Oeffentlich:</label>
                    <input
                        id="Post-text"
                        defaultChecked={postPublic}
                        onChange={(e) => setPublic(e.target.checked)}

                        type="checkbox"
                    />
                        { postRetzwitscher !== "" &&(
                         <><label>Post ist Retzwitscher:</label><input
                            id="Post ist Retzwitscher"
                            defaultChecked={postRetzwitscher !== ""}
                            onChange={(e) => setRetzwitscher(e.target.checked)}
                            placeholder="Text..."
                            type="checkbox" /></>
                        )}
                        
                         <br/>
                        <Button
                            onClick={handleSubmit}

                            
                        >
                            Speichern
                        </Button>
                    </div>
                
                
            </div></>


    );
}

export default EditPostDialog;