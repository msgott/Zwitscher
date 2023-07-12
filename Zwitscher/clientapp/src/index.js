import React from 'react';
import {createRoot }from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { BrowserRouter } from 'react-router-dom';
import CookieConsent from "react-cookie-consent";
const root = createRoot(document.getElementById('root'));
root.render(
    
            <React.StrictMode>
                <BrowserRouter basename='/Zwitscher'>
            <App />
            <CookieConsent
                location="bottom"
                overlay = "true"
                buttonText="Geht klar!"
            >This website uses cookies to enhance the user experience.</CookieConsent>

                </BrowserRouter>
            </React.StrictMode>
        ,
);

reportWebVitals();
