import { initializeApp } from "firebase/app";
import {getFirestore} from 'firebase/firestore';

const firebaseConfig = {
  apiKey: "AIzaSyBC3S4XjBQ1JWQPR8sfhr_1F5feoBShv0s",
  authDomain: "zwitscher-23fb0.firebaseapp.com",
  projectId: "zwitscher-23fb0",
  storageBucket: "zwitscher-23fb0.appspot.com",
  messagingSenderId: "65595218877",
  appId: "1:65595218877:web:d4fbb5081b17f5b5b510d4"
};

const app = initializeApp(firebaseConfig);

export const db = getFirestore(app);