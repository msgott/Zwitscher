import React, { useState, useEffect } from "react";
import "./ZwitscherBox.css";
import Button from "@mui/material/Button";
import Avatar from "@mui/material/Avatar";
import ImageIcon from "@mui/icons-material/Image";
import VideocamIcon from "@mui/icons-material/Videocam";

function ZwitscherBox() {
  const [zwitscherMessage, setZwitscherMessage] = useState("");
  const [zwitscherImage, setZwitscherImage] = useState("");

  const sendZwitscher = async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("files", zwitscherImage); // Assuming zwitscherImage is a file object

    const postPayload = {
      TextContent: zwitscherMessage,
    };

    formData.append("post", JSON.stringify(postPayload));

    try {
      const response = await fetch("https://localhost:7160/API/Posts/Add", {
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        // Post created successfully
        console.log("Zwitscher posted successfully!");
        setZwitscherMessage("");
        setZwitscherImage("");
      } else {
        // Handle the error case
        console.error("Failed to post Zwitscher:", response.status);
      }
    } catch (error) {
      console.error("Error posting Zwitscher:", error);
    }
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

  useEffect(() => {
    // Get Avatar image depending on the filename using import() Method
    const importImage = async () => {
      try {
        const module = await import(`../../wwwroot/Media/${pbFileName}`);
        const image = module.default;
        setPbFileName(image);
      } catch (error) {
        console.error("Error importing image:", error);
      }
    };

    importImage();
  }, [pbFileName]);

  return (
    <div className="zwitscherBox">
      <form>
        <div className="zwitscherBox_input">
          <Avatar src={pbFileName}></Avatar>
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
