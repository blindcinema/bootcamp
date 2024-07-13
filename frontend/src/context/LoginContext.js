import React,{ useEffect, useState } from "react";

export const LoginContext = React.createContext({});
export const LoginContextConsumer = LoginContext.Consumer;
export function LoginContextProvider({children}) {
    const [user,setUser] = useState({id:"",name:"",username:"",role:"",token:""})
    const [isLoggedIn,setIsLoggedIn]=useState(false)
    useEffect(()=>{
        if (localStorage.getItem("user")) {
            const user = JSON.parse(localStorage.getItem("user"));
            setUser(user);
            setIsLoggedIn(true);
        }
        else {
            setIsLoggedIn(false);
            
        }
    },[localStorage.getItem("user")]);
    return (
        <LoginContext.Provider value={{user, setUser, isLoggedIn, setIsLoggedIn}}>
            {children}
        </LoginContext.Provider>
    )
};  