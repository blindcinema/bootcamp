import { useContext, useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import { getAllRoles, getAllUsers, requestRole } from "../services/userService";
import Form from "react-bootstrap/Form";
import { LoginContext } from "../context/LoginContext";
import { updateUser } from "../services/clientService";


export default function UserProfile() {
    let loginContext = useContext(LoginContext);
    const localUser = localStorage.getItem("user");
    const [profile,setProfile] = useState(loginContext.user);
    const [roles,setRoles] = useState([]);
    const [form,setFormInputs] = useState({}); 

    const handleFormChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        setFormInputs(values => ({...values, [name]: value}))

      }
    async function handleEditSubmit(e) {
        e.preventDefault();
        console.log(form);
        await updateUser({token: JSON.parse(localUser).token,id: JSON.parse(localUser).id ,name: form.name,phone: form.phone, email: form.email, username: form.username});
    }
    function handleRequestRole(e) {
        const userData = JSON.parse(localUser);
        const form = e.target;
        const formData = new FormData(form);
        //console.log(formData.get("requestedRole"));
        e.preventDefault();
        requestRole(userData.token,userData.id,formData.get("requestedRole"));
    }

    useEffect(()=>{
        setProfile(loginContext.user);
        const fetchRoles = async ()=> {
            const result = await getAllRoles();
            setRoles(result.data);
                
        }
        fetchRoles();
        return () =>{
        }
    },[loginContext.isLoggedIn])
    return (
        <>
        <Navbar/>
        <main>
        {!loginContext.isLoggedIn ? <div>Please Log In</div> : <>
        {roles.length === 0 ? <div>Please wait...</div> :
            <div className="flex justify-center flex-col items-center">
                    

                    
                     <div>
                        <div>Your name: {profile.name}</div>
                        <div>Your username: {profile.userName}</div>
                        <div>Your email: {profile.email}</div>
                    </div>
                    <div>
                        <form onSubmit={handleEditSubmit} className="flex flex-col mt-8 mb-12">
                        <p>Edit user info</p>
                            <label htmlFor="name">Name</label>
                            <input name="name" onChange={handleFormChange}></input>
                            <label htmlFor="email">Email</label>
                            <input type="email" name="email" onChange={handleFormChange}></input>
                            <label htmlFor="phone">Phone</label>
                            <input name="phone" onChange={handleFormChange}></input>
                            <label htmlFor="username">username</label>
                            <input name="username" onChange={handleFormChange}></input>
                            <button type="submit" >Submit</button>
                        </form>
    
                    <div className="w-max flex justify-center">
                        <form onSubmit={handleRequestRole}>
                            <label htmlFor="requestedRole">Request a Role:</label>
                            <Form.Select defaultValue={"Client"} name="requestedRole">
                                <option disabled>Choose a role</option>
                                <option value={roles[2].id}>Courier</option>
                                <option value={roles[1].id}>Sender</option>
                            </Form.Select>
                        <button type="submit">Request role</button>
                        </form>
                    </div>
                    </div>
                </div> 
                
                } 
                </>}    
        </main>
        </>
    )
}