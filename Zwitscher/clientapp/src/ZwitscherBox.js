import React, {useState} from 'react';
import './ZwitscherBox.css';
import Button from '@mui/material/Button';
import Avatar from '@mui/material/Avatar';
{/*
import {db} from './Firebase';
import { getDocs, collection, addDoc } from "firebase/firestore";
*/}


function ZwitscherBox() {
  const [zwitscherMessage, setZwitscherMessage] = useState('');
  const [zwitscherImage, setZwitscherImage] = useState('');

  {/*With firebase:

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
*/}
  
  

  return (
    <div className="zwitscherBox">
        <form>
            <div className="zwitscherBox_input">
                <Avatar src="https://www.thesprucepets.com/thmb/APYdMl_MTqwODmH4dDqaY5q0UoE=/750x0/filters:no_upscale():max_bytes(150000):strip_icc():format(webp)/all-about-tabby-cats-552489-hero-a23a9118af8c477b914a0a1570d4f787.jpg"></Avatar>
                <input onChange={(e) => setZwitscherMessage(e.target.value)}
                 value={zwitscherMessage}
                 placeholder="What's going on?"
                 type="text" />
            </div>

            {/*Input field connected to the useState method to set and assign the value for "zwitscherImage". An Image link will be 
            provided here and will be stored

            <input
             onChange={(e) => setZwitscherImage(e.target.value)}
             className="zwitscherBox_imageInput"
             value={zwitscherImage}
             placeholder='optional: image'
             type='text' />
            
            */}
            
            {/*Less functional input field for Images, DUMMY*/}
            <input className="zwitscherBox_imageInput" placeholder="Optional: Enter image URL" type="text"/>

            {/*sendZwitscher function call to create a new "zwitscher/message/tweed". 
            It will also be visable in the database after the function call. The Author,
            Image etc. still hardcoded at this point*/}
            
            {/*<Button onClick={sendZwitscher} type='submit' className="zwitscherBox_zwitscherButton">Zwitscher</Button>*/}
        </form>
    </div>
  );
}

export default ZwitscherBox;
