import React, { useState, useEffect } from "react";
import "./Comments.css";

function Comments() {
  const [data, setData] = useState([]);

  {
    /*Get data from the Database/API */
  }
  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("https://localhost:7160/API/Comments"); // Replace with your API endpoint
        const jsonData = await response.json();
        setData(jsonData);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);

  return (
    <div>
      <h1>Hello There!!!</h1>
    </div>
  );
}

export default Comments;
