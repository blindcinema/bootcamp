import { useContext, useEffect, useState } from "react";
import { ThemeContext } from "../context/ThemeContext";
import { WiMoonAltWaxingCrescent1, WiMoonAltWaningCrescent4 } from "react-icons/wi";
import { LoginModal } from "./LoginModal";
import { LoginContext } from "../context/LoginContext";
import { logout } from "../services/userService";
import { Link, useNavigate } from "react-router-dom";
export default function Navbar() {
    const navigate = useNavigate();
    const [contextC,setContextC] = useState(localStorage.getItem("user"));
    let loginContext = useContext(LoginContext);
    let themeContext = useContext(ThemeContext);
    const [nav,setNav] = useState("");
    useEffect(()=> {
      document.body.setAttribute("data-theme",themeContext.theme);
    },[themeContext])
    
    function themeSwitch() {
      if (themeContext.theme === "dark") {
    
        themeContext.setTheme("light");
      }
      else {
        themeContext.setTheme("dark");
      }
    }
    const [isOpen,setIsOpen] = useState(false);
    function handleModal() {
      setIsOpen(!isOpen);
    }
    function handleLogout() {
      navigate("/");
        logout();
        loginContext.setIsLoggedIn(false);
        setNav("dont work")
        
    }
    function roleBasedNav() {
      switch(loginContext.user.role) {
        case "Client":
          return (
            <>
            <li>
              <Link to="/profile" className="hover:underline">Profile</Link>
            </li>
            <li>
            <Link to="/client" className="hover:underline">Tracking</Link>
            </li>
            <li>
            <Link to="/packages" className="hover:underline">Delivered packages</Link>
            </li>
            </>  
          )
        case "Admin":
          return (
            <>
            <li>
            <Link to="/dashboard" className="hover:underline">Admin Dashboard</Link>
            </li>
            </>  
          )
          case "Sender":
            return (
              <>
              <li>
              <Link to="/sender" className="hover:underline">Create a package</Link>
              </li>
              </>  
            )
          case "Courier":
            return (
              <>
              <li>
              <Link to="/courierpackagesintransit" className="hover:underline">Packages in transit</Link>
              </li>
              <li>
              <Link to="/courierpackagespending" className="hover:underline">Pending packages</Link>
              </li>
              </>  
            )
          default:
            return (
              <Link to="/" className="hover:underline">Home</Link>
            )
      }
    }

    useEffect(()=>{
      if (!loginContext.isLoggedIn) {
        setNav(<Link to="/" className="hover:underline">Home</Link>)
      } else {
        setNav(roleBasedNav());
      }
      
      
    },[loginContext.user.role, loginContext.isLoggedIn])
    
    return (
        <nav className="flex justify-between bg-[--secondary] text-[#c9dbe8] flex-row mb-10">
      <div className="mt-2 justify-self-start">
      <button onClick={themeSwitch} className="text-2xl ">
    {themeContext.theme =="dark" ? <WiMoonAltWaningCrescent4 /> :  <WiMoonAltWaxingCrescent1 />}
  </button>
      </div>
    <ul className="inline-flex gap-4 mt-2 ml-2 " >
      {nav}
    </ul>
    <div className="mt-2 mr-4 ml-2">
      {!loginContext.isLoggedIn ? <button className="hover:underline" id="loginModalBtn" onClick={handleModal}> 
    Login/Register
  </button>
  : <button onClick={handleLogout} className="hover:underline w-max pl-2 pr-2 rounded-sm ">Logout</button>}
      </div>
      {// check in context if user is logged in, if not show modal, if yes: Welcome "Name"
      }
      {!loginContext.isLoggedIn ? <LoginModal handleModal={handleModal} isOpen={isOpen} setIsOpen={setIsOpen}/> : "" }
      
    </nav>
    );
};