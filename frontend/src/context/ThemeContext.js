import React,{ useContext, useEffect, useState } from "react";

export const ThemeContext = React.createContext("dark");
export const ThemeContextConsumer = ThemeContext.Consumer;
export function ThemeContextProvider({children}) {
    const context = useContext(ThemeContext);
    const [theme,setTheme] = useState(localStorage.getItem("theme") || context);
    useEffect(()=> {
        localStorage.setItem("theme",theme)
        
    },[theme])

    
    return (
        <ThemeContext.Provider value={{theme,setTheme}}>
            {children}
        </ThemeContext.Provider>
    )
};  