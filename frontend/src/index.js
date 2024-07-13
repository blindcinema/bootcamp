import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import "./index.css"
import reportWebVitals from './reportWebVitals';
import { ThemeContextProvider } from './context/ThemeContext';
import { Route, RouterProvider, createBrowserRouter } from "react-router-dom";
import ClientPackageReceived from './routes/ClientPackageReceved';
import CreatePackage from './routes/CreatePackage';
import SenderPackage from './routes/SenderPackage';
import { LoginContextProvider } from './context/LoginContext';
import { Register } from './routes/Register';
import UserProfile from './routes/UserProfile';
import AdminDashboard from './routes/AdminDashboard';
import ClientPackage from './routes/ClientPackage';
import CourierPackagesPending from './routes/CourierPackagesPending';
import CourierPackagesInTransit from './routes/CourierPackagesInTransit';



const router = createBrowserRouter([
  {path: "/", element:<App></App>},
  {path: "/register",element: <Register/>},
  {path:"/profile",element:<UserProfile/>},
  {path:"/packages",element:<ClientPackageReceived/>},
  {path:"/createPackage",element:<CreatePackage/>},
  {path:"/Sender",element:<SenderPackage/>},
  {path:"/dashboard",element:<AdminDashboard/>},
  {path:"/client",element:<ClientPackage/>},
  {path:"/CourierPackagesPending",element:<CourierPackagesPending/>},
  {path:"/CourierPackagesInTransit",element:<CourierPackagesInTransit/>}

]);
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    
    <ThemeContextProvider>
    <LoginContextProvider>
    <RouterProvider router={router} />
    </LoginContextProvider>
    </ThemeContextProvider>
    
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
