import React from 'react';
import {Link} from 'react-router-dom';

function Error() {
  return (
    <div>
      <h1>404: The requested resource was not found.</h1>
    <Link to='/'>Zurück zu Zwitscher</Link>
    </div>
  )
}

export default Error
