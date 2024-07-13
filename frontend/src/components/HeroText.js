import { useContext, useEffect, useState } from "react";
import { LoginContext } from "../context/LoginContext";


export default function HeroText() {
const loginContext = useContext(LoginContext);

       
    const [dots,setDots] =useState("");
    const [emojis,setEmojis] = useState("📭");
    const flagger = ()=> {
        if (emojis === "📭"){
            setEmojis("📬");
        }
        else {
            setEmojis("📭");
        }
    }
    const dotter = () => {
        if (dots.length < 3) {
            setDots(dots.concat("","."));
        }
        else {
            setDots("");
        }

    }
    useEffect(()=> {
        const intv =setInterval(()=> {dotter();flagger()},1000);
        return () => clearInterval(intv);
    },[dots])

return (
<div className="block m-auto w-[50%] text-center">
<h1 className="text-4xl font-Playwrite">
  TrackIt!<span>{}</span>
</h1>
<div className="p-4 mt-12">
 {!loginContext.isLoggedIn ? <p>Log In to begin.</p> : <p>Welcome, {loginContext.user.name}.</p>}
 
<div className="flex justify-center scale-50 ">
    
    <img src="https://img.icons8.com/?size=400&id=NFRIR5scaUFk&format=png"></img>
</div>
</div>


</div>
);
};
