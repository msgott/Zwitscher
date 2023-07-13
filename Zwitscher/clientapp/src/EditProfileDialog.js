import React, { useContext, useState, useEffect } from "react";
import "./EditProfileDialog.css";

import Avatar from "@mui/material/Avatar";

import { Button, Modal } from "@mui/material";
import { useNavigate } from "react-router-dom";




function EditProfileDialog({
    userObject,
    handleClose,
    setuserCounter
}) {
    //Modal stuff------------------------------------------------
    const [EditProfileDeleteOpen, setEditProfileDeleteOpen] = React.useState(false);
    const EditProfileDeletehandleOpen = () => setEditProfileDeleteOpen(true);
    const EditProfileDeletehandleClose = () => setEditProfileDeleteOpen(false);


    // Main File to load all the Components on the page (Header, Sidebar, Feed etc.)

    // set the theme to 'light mode' in the beginning and have the opportunity to change theme
    // depending on toggleTheme
    console.log(userObject);
    const [theme, setTheme] = useState("light");

    const toggleTheme = () => {
        setTheme((curr) => (curr === "light" ? "dark" : "light"));
    };
    // Navigate to the profile page if set to true. Follow goToProfileContext.Provider to understand
    // routing with React v18

    const [goToProfile, setGoToProfile] = useState(false);
    /*const navigate = useNavigate();*/

    // Get all users information and session data from the current logged-in user
    const [usersData, setUsersData] = useState([]);
    const [sessionData, setSessionData] = useState([]);
    const [pbFileName, setPbFileName] = useState("");

    // Persons attribute assignment React
    const [userId, setUserId] = useState("");
    const [firstname, setFirstname] = useState("");
    const [lastname, setLastname] = useState("");
    const [username, setUsername] = useState("");
    const [birthday, setBirthday] = useState("");
    const [biography, setBiography] = useState("");
    const [followedCount, setFollowedCount] = useState(0);
    const [followerCount, setFollowerCount] = useState(0);
    const [gender, setGender] = useState("");
    const [createdDate, setcreatedDate] = useState("");
    const [password, setPassword] = useState("");
    const [file, setFile] = useState(null);


    // Persons PostCount
    const [postCount, setPostCount] = useState(0);

    useEffect(() => {
        const setdefaultValues = async () => {
            try {
                if (userObject != undefined) {

      
                    if (!userObject.pbFileName) {

                        setPbFileName("real-placeholder.png");



                    } else {

                        setPbFileName(userObject.pbFileName);
                    };







                    setUserId(userObject ? userObject.userID : "");
                    setFirstname(userObject ? userObject.firstname : "");
                    setLastname(userObject ? userObject.lastname : "");
                    setUsername(userObject ? userObject.username : "")
                    setBirthday(userObject ? userObject.birthday : "");
                    setBiography(userObject ? userObject.biography : "");
                    setFollowedCount(userObject ? userObject.followedCount : 0);
                    setFollowerCount(userObject ? userObject.followerCount : 0);
                    setcreatedDate(userObject ? userObject.createdDate : "");


                   

                        if (userObject.gender === "Maennlich") setGender(0);
                        if (userObject.gender === "Weiblich") setGender(1);
                        if (userObject.gender === "Divers") setGender(2);

                    




                }
            } catch (error) {
                console.error("Error setting data:", error);
            }
        };

        setdefaultValues();
    }, []);

    const handleFileChange = (e) => {
        if (e.target.files) {
            setFile(e.target.files[0]);
        }
    };
    const handleSubmit = async (e) => {
        e.preventDefault();



        // Prevent the default form submission behavior
        // Perform any necessary actions here, such as saving the form data or making API calls
        var requestOptions = {
            method: 'POST',
            //body: JSON.stringify({
            //    "userID": userId,
            //   "LastName": lastname,
            //    "FirstName": firstname,
            //    "Username": username,
            //    "Password": password,
            //    "Birthday": birthday,
            //    "Biography": biography,
            //    "Gender": gender
            /*}),*/
            redirect: 'follow'
        };


        var response = await fetch("https://localhost:7160/API/Users/Edit?userID=" + userId + "&LastName=" + lastname + "&FirstName=" + firstname + "&Username=" + username + "&Password=" + password + "&Birthday=" + birthday + "&Biography=" + biography + "&Gender=" + gender, requestOptions)
            

        if (file != null) {

            var formdata = new FormData();
            formdata.append("userID", userId);
            formdata.append("file", file);

            var requestOptions = {
                method: 'POST',
                body: formdata,
                redirect: 'follow'
            };
            response = await fetch('https://localhost:7160/API/Users/Media/Add', requestOptions)
                

        }
        if (response.ok) {
            handleClose();
            setuserCounter(Math.random);
        } else {
            window.location.reload();
        }
    };
    const navigate = useNavigate();
    const  handleDelete = async () => {
       



        // Prevent the default form submission behavior
        // Perform any necessary actions here, such as saving the form data or making API calls
        var requestOptions = {
            method: 'DELETE',
            //body: JSON.stringify({
            //    "userID": userId,
            //   "LastName": lastname,
            //    "FirstName": firstname,
            //    "Username": username,
            //    "Password": password,
            //    "Birthday": birthday,
            //    "Biography": biography,
            //    "Gender": gender
            /*}),*/
            redirect: 'follow'
        };


        var response = await fetch("https://localhost:7160/API/Users/Remove?id="+userId, requestOptions)
            .then(response => response.text())
            .then(result => console.log(result))
            .catch(error => console.log('error', error));
        
        window.location.href = "https://localhost:7160/Auth";
        
           
        
        handleClose();
    };

    return (
        // It matters here which component comes first. Flux model not mvc. 1.ThemeContext gives theme to all data/components/ underneath, 2. goToProfile all to the lower components and so on
        <>
            
            <div Class="form-container">
                <h1 Class="EditFormUsername">{username}</h1>
                <img Class="EditFormAvatar" src={"/Media/" + pbFileName}></img>


                <form id="profileform">
                    <input type="hidden" name="userID" value={userId} />
                    <div>
                        <label>Profilbild Aendern:</label>

                        <input type='file' id='file' accept='image/png, image/gif, image/jpeg' onChange={handleFileChange} />
                        <label for="nameinput">Name:</label>
                        <input
                            id="nameinput"
                            value={userObject.firstname}
                            onChange={(e) => setFirstname(e.target.value)}
                            placeholder="Vorname..."
                            type="text"

                            name="FirstName" />


                        <label>Lastname:</label>
                        <input
                            value={lastname}
                            onChange={(e) => setLastname(e.target.value)}
                            placeholder="Nachname..."
                            type="text"

                            name="LastName" />

                        <input type="hidden" name="Username" value={username} />

                        <label>About:</label>
                        <input
                            value={biography}
                            onChange={(e) => setBiography(e.target.value)}
                            placeholder="Biographie"
                            type="text"

                            name="Biographie" />

                        <label>Birthday: </label>
                        <input
                            value={birthday}
                            onChange={(e) => setBirthday(e.target.value)}
                            placeholder="01.01.2000"
                            type="text"

                            name="Geburtstag" />
                        <label>Geschlecht: </label>
                        <select
                            value={gender}
                            onChange={(e) => setGender(parseInt(e.target.value))}

                            name="Gender"
                        >
                            <option value="" disabled>Geschlecht auswaehlen</option>
                            <option value="0">Maennlich</option>
                            <option value="1">Weiblich</option>
                            <option value="2">Divers</option>
                        </select>
                        <label>Passwort: </label>
                        <input
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            type="password"
                            placeholder="neues Passwort"

                            name="Password" />

                        <Button
                            onClick={handleSubmit}

                            type="submit"
                        >
                            Speichern
                        </Button>
                    </div>
                </form>
                <Button Class="deleteAccountButton" onClick={() => { EditProfileDeletehandleOpen() } }>Account loeschen</Button>
            </div><Modal
                open={EditProfileDeleteOpen}
                onClose={EditProfileDeletehandleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"
            >


                <div className="DeleteDialog">
                    <p>Account wirklich loeschen?</p>
                    <Button onClick={() => { handleDelete() } }>Ja, Loeschen</Button>
                </div>


            </Modal></>


    );
}

export default EditProfileDialog;