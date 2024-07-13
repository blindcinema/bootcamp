import { useContext, useRef, useState } from "react";
import { login } from "../services/userService";
import { IoMdCloseCircleOutline } from "react-icons/io";
import ModalTemplate from "./ModalTemplate";
import { LoginContext } from "../context/LoginContext";


export function LoginModal({handleModal, isOpen}) {
    const formRef = useRef(null);
    let loginContext = useContext(LoginContext);
    const [formInputs, setFormInputs] = useState({});
    const handleFormChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        setFormInputs(values => ({...values, [name]: value}))

      }
      
  async function handleLoginSubmit(e) {
    e.preventDefault();
    const response = await login(formInputs);
    if (response)
        {
            loginContext.setUser({id:response.id,name:response.name,username:response.userName,role:response.role})
            loginContext.setIsLoggedIn(true);
        }

  }
  function handleKey(e) {
    if (e.key ==="Escape") {
        handleModal();
    }
    if (e.key === "Enter") {
        
        handleLoginSubmit(e);
    }

}

    return (

        <ModalTemplate isOpen={isOpen} onClose={handleModal} className="flex bg-[--background] text-[--text] rounded-lg backdrop-blur-lg " onkeydown={handleKey}>
      
         {isOpen &&
         
                 <form ref={formRef} id="loginForm" className="flex flex-shrink flex-row pb-4 p-8 pl-16 pr-16 bg-[--background2] rounded-lg " onSubmit={handleLoginSubmit} >
                  <button onClick={handleModal} className="absolute right-3 top-3 text-2xl "><IoMdCloseCircleOutline /></button> 
                 <div className="flex flex-col h-max w-max gap-4">
                 
                 <label htmlFor="username" >Username: </label>
                 <input type="text" id="username" name="username" className="bg-[--background]" value={formInputs.formName} onChange={handleFormChange}/>
                 <label htmlFor="password">Password: </label>
                 <input type="password" id="password" name="password" className="bg-[--background]" value={formInputs.formPassword} onChange={handleFormChange}/>
                 <button type="submit" className="mt-4 bg-[--background] rounded-2xl h-8 hover:bg-[--accent]">Log In</button>
                 <a href="/register" className="text-xs mt-4 hover:text-[--accent]">Register</a>
                 </div>
               </form> } 

      </ModalTemplate>
    )
}