import React, { useState } from "react";
import "./ZwitscherBox.css";
import Button from "@mui/material/Button";
import Avatar from "@mui/material/Avatar";
{
  /*
import {db} from './Firebase';
import { getDocs, collection, addDoc } from "firebase/firestore";
*/
}

function ZwitscherBox() {
  const [zwitscherMessage, setZwitscherMessage] = useState("");
  const [zwitscherImage, setZwitscherImage] = useState("");

  {
    /*With firebase:

    const postsCollectionRef = collection(db, "posts");

    const sendZwitscher = async () => {
      try {
        await addDoc(postsCollectionRef, {
          name: "JD",
          text: zwitscherMessage,
          image: zwitscherImage,
          avatar:"https://upload.wikimedia.org/wikipedia/en/a/a2/Jd_season9.jpg" ,
        });
      } catch(err){
        console.error(err);
      }
      
      setZwitscherMessage("");
      setZwitscherImage("");
    }
*/
  }

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

  return (
    <div className="zwitscherBox">
      <form>
        <div className="zwitscherBox_input">
          <Avatar src="https://www.thesprucepets.com/thmb/APYdMl_MTqwODmH4dDqaY5q0UoE=/750x0/filters:no_upscale():max_bytes(150000):strip_icc():format(webp)/all-about-tabby-cats-552489-hero-a23a9118af8c477b914a0a1570d4f787.jpg"></Avatar>

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
          <input
            onChange={(e) => setZwitscherImage(e.target.value)}
            className="zwitscherBox_imageInput"
            value={zwitscherImage}
            placeholder="optional: image"
            type="text"
          />

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
