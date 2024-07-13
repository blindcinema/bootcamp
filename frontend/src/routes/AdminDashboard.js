import { useContext, useEffect, useState } from "react"
import { ThemeContext } from "../context/ThemeContext";
import Navbar from "../components/Navbar";
import { approveUser, editUser, getAllUsers, getUnapprovedUsers, getUserById } from "../services/userService";
import { redirect, useNavigate } from "react-router-dom";
import { LoginContext } from "../context/LoginContext";


export default function AdminDashboard() {
    let loginContext = useContext(LoginContext);
    const [data,setData] = useState([]);
    const [form,setForm] = useState({});
    const [userId,setUserId] = useState("");
    const navigate = useNavigate;
    const [token,setToken]= useState(JSON.parse(localStorage.getItem("user")).token);
    
    useEffect(()=> 
        { if(localStorage.getItem("user") === null) {
            return () => {
                navigate("/")
                alert("log in or its gonna break")
            }
        } else {setToken(JSON.parse(localStorage.getItem("user")).token)}


        },[])
        


    const localGetAllUsers = async () => {
        const users = await getAllUsers(token);
        setData(users.data);
    }

    const localGetUserById = async () => {
        const id = JSON.parse(localStorage.getItem("user")).id;
        const user = await getUserById(token,id);
        //console.log(user);
        setData([]);
        setData([user.data]);
    }
    const localEditUser = async(e) => {
        e.preventDefault();

        
         const user = await editUser(token,form);
         //console.log(user.data)
        //setData(user.data);
    }
    const localGetUnapprovedUsers = async () => {
        const user = await getUnapprovedUsers(token);
        console.log(user.data.users);
        setData([]);
        setData(user.data.users);
        
    }
    const localApproveUser = async() => {
        
        await approveUser(token,userId);
    }
    const Btn = () =>(
        <div>
        </div>
    )
    function handleChange(e) {
        const name = e.target.name;
        const value = e.target.value;
        setForm(values => ({...values, [name]: value}));
        
        
    }
    const style = "bg-[--background2] rounded p-2 m-2 hover:brightness-150";
    const themeContext = useContext(ThemeContext);

    return (
        <>
        <Navbar/>
        <div className="flex flex-row w-full">
            <div className="background-[--background] flex flex-col">
                <button className={style} onClick={localGetAllUsers}>getallusers</button>
                <button className={style} onClick={localGetUserById}>getuserbyid</button>
                
                <button className={style} onClick={localGetUnapprovedUsers}>getunapprovedusers</button>
                <input type="text" onChange={(e) => setUserId(e.target.value)}></input>
                <button className={style} onClick={localApproveUser}>approveuser</button>
            </div>
            <div className="flex flex-col m-4">
                    <ul>
                    {data.map((user)=>(
                        
                        <li key={user.id}>{user.name}  {user.id} </li>
                        
                    ))}
                    </ul>
                </div>
            </div>
        </>


    )

}