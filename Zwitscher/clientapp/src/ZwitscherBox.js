import React, { useState, useEffect } from "react";
import "./ZwitscherBox.css";
import Button from "@mui/material/Button";
import Avatar from "@mui/material/Avatar";
import ImageIcon from "@mui/icons-material/Image";
import VideocamIcon from "@mui/icons-material/Videocam";

function ZwitscherBox() {
    const [zwitscherMessage, setZwitscherMessage] = useState("");
    const [files, setFiles] = useState(Array.from([]));


    const sendZwitscher = async (e) => {
        e.preventDefault();
        if (zwitscherMessage.length == 0) {
            alert("Gebe bitte eine Textnachricht ein");
            return;
        }
        var requestOptions = {
            method: 'POST',
            redirect: 'follow'
        };

        var formdata = new FormData();
        formdata.append("TextContent", zwitscherMessage);
        formdata.append("IsPublic", true);

        if (files != null) {
            files.map((file) => {
                formdata.append("files", file);
            })
            

        }
        var requestOptions = {
            method: 'POST',
            body: formdata,
            redirect: 'follow'
        };
        fetch('https://localhost:7160/API/Posts/Add', requestOptions)
            .then((res) => res.json())
            .then((data) => console.log(data))
            .catch((err) => console.error(err));
        setZwitscherMessage("");
        setFiles(Array.from([]));
        document.getElementById("files").value = null;

    };

    // Get all users information and session data from the current logged-in user
    const [usersData, setUsersData] = useState([]);
    const [sessionData, setSessionData] = useState([]);
    const [pbFileName, setPbFileName] = useState("");

    useEffect(() => {
        const fetchData = async () => {
            try {
                // Fetch users data
                const usersResponse = await fetch("https://localhost:7160/API/Users");
                const usersJsonData = await usersResponse.json();
                setUsersData(usersJsonData);

                // Fetch session data
                const sessionResponse = await fetch("https://localhost:7160/Api/UserDetails");
                const sessionJsonData = await sessionResponse.json();
                setSessionData(sessionJsonData);

                const currentUser = sessionJsonData.Username;

                // Check if the current user is in the list of all registered users and then retrieve the filePath from that API
                const currentUserData = usersJsonData.find(
                    (user) => user.username === currentUser
                );

                if (currentUserData) {
                    setPbFileName(currentUserData.pbFileName);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();
    }, []);

    

    const handleFileChange = (e) => {
        if (e.target.files) {
            setFiles(Array.from(e.target.files));
        }
    };
    
    return (
        <div className="zwitscherBox">
            <form>
                <div className="zwitscherBox_input">
                    <Avatar src={"/Media/"+pbFileName}></Avatar>
                    {/*Text Input*/}
                    <input
                        onChange={(e) => setZwitscherMessage(e.target.value)}
                        value={zwitscherMessage}
                        placeholder="What's going on?"
                        type="text"
                    />
                </div>

                <div className="zwitscherbox_footer">
                    {/*Image Input*/}
                    <div className="zwitscherbox_footerLeft">
                        <ImageIcon className="zwitscherBox_imageInput" />
                        <input type='file' id='files' multiple="multiple" accept='image/png, image/gif, image/jpeg, video/mp4' onChange={handleFileChange} />

                        <VideocamIcon className="zwitscherBox_videoInput" />
                    </div>
                    {/*Submit all Inputs*/}
                    <Button
                        onClick={sendZwitscher}
                        type="submit"
                        className="zwitscherBox_zwitscherButton"
                    >
                        Zwitscher
                    </Button>
                </div>
            </form>
        </div>
    );
}

export default ZwitscherBox;
