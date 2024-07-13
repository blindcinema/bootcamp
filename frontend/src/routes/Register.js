import { useContext, useEffect, useState } from "react";
import { ThemeContext } from "../context/ThemeContext";
import { register } from "../services/userService";
import { useNavigate } from "react-router-dom";
import { LoginContext } from "../context/LoginContext";


export function Register() {
    const navigate = useNavigate();
    let loginContext = useContext(LoginContext);
    let themeContext = useContext(ThemeContext);
    const [formInputs, setFormInputs] = useState({});
    useEffect(()=> {
      document.body.setAttribute("data-theme",themeContext.theme);
    },[themeContext])
    async function handleSubmit(e) {
        e.preventDefault()
        
        const reg = await register(formInputs);
        console.log(reg);
        localStorage.setItem("user", JSON.stringify(reg));
        loginContext.setUser(reg);
        navigate("/");
    }
    function handleChange(e) {
        const name = e.target.name;
        const value = e.target.value;
        setFormInputs(values => ({...values, [name]: value}));


    }

    return (
        <>
        
        <div className="bg-[--background] h-screen w-full">
            <div className="flex w-screen h-screen items-center justify-center ">
            <form onSubmit={handleSubmit} className="flex flex-col w-max pl-12 pr-12 pt-12 pb-10 bg-[--background2] rounded-2xl gap-2">
                  
                <label htmlFor="name">Name:</label>
                <input type="text" id="name" name="name" onChange={handleChange} className="bg-[--background]" required/>
                <label htmlFor="username">Username:</label>
                <input type="text" id="username" name="username" onChange={handleChange} className="bg-[--background]" required/>
                <label htmlFor="email">Email:</label>
                <input type="email" id="email" name="email" onChange={handleChange} className="bg-[--background]" required/>
                <label htmlFor="password">Password:</label>
                <input type="password" id="password" name="password" onChange={handleChange} className="bg-[--background]" required/>
                
                <div className="w-full flex justify-center">
                    <button type="submit" className="mt-4 bg-[--background] rounded-xl pb-2 pt-2 pl-4 pr-4 hover:bg-[--accent] w-min justify-self-center">Register</button>
                </div>
            </form>
            </div>
        </div>
        </>
    )
}