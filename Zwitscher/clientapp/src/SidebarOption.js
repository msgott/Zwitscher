import React from 'react'
import "./SidebarOption.css";
import { useNavigate } from 'react-router-dom';

{/* When you are active the color of the selected active sidebar will change to zwitscher-color */}

function SidebarOption({selected, text, Icon}) {
  return (
    <div className={`sidebarOption ${selected && 'sidebarOption--selected'}`}>
        <Icon />
        <h2>{text}</h2>
      
    </div>
  )
}

export default SidebarOption
