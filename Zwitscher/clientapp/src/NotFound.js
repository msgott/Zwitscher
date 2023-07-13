import React,{useEffect} from 'react'
import { useNavigate } from 'react-router-dom';

function NotFound() {

    const navigate = useNavigate();

    useEffect(()=>{
        setTimeout(()=>{
            navigate("/")
        }, 1000)
    })
  return (
    <div>
      <h1>Not Found 404</h1>
    </div>
  )
}

export default NotFound
