import React from 'react';
import './ZwitscherBox.css';
import Button from '@mui/material/Button';
import Avatar from '@mui/material/Avatar';


function ZwitscherBox() {
  return (
    <div className="zwitscherBox">
        <form>
            <div className="zwitscherBox_input">
                <Avatar src="https://www.thesprucepets.com/thmb/APYdMl_MTqwODmH4dDqaY5q0UoE=/750x0/filters:no_upscale():max_bytes(150000):strip_icc():format(webp)/all-about-tabby-cats-552489-hero-a23a9118af8c477b914a0a1570d4f787.jpg"></Avatar>
                <input placeholder="What is happenenig?" type="text" />
            </div>
            <input className="zwitscherBox_imageInput" placeholder="Optional: Enter image URL" type="text"/>
            <Button className="zwitscherBox_zwitscherButton">Zwitscher</Button>
        </form>
    </div>
  );
}

export default ZwitscherBox;
