import React from 'react'
import { Link,Navigate } from 'react-router-dom'

function Home() {
  return (
    <div>
        <Navigate to="/about" />
      Home Page
      <Link to ="about">Got to the about page</Link>
    </div>
  )
}

export default Home
