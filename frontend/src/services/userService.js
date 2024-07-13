import { useContext, useState } from "react";
import { LoginContext } from "../context/LoginContext";
import axios from "axios";

export async function register(username,password,name,email) {
    return await axios.post('https://localhost:7274/api/User/register',username,password,name,email)
    .then(response => {
        
        if (response.status === 200) {
            response = response.data;
            
            localStorage.setItem("user",JSON.stringify(response));
            
            return response;
            //console.log(response);
        }
        else {
            alert("a");
            return null;
        }
    })
};
export async function login(username,password) {
    return await axios.post('https://localhost:7274/api/User/login',username,password)
    .then(response => {
        
        if (response.status === 200) {
            response = response.data;
            
            localStorage.setItem("user",JSON.stringify(response));
            
            return response;
            //console.log(response);
        }
        else {
            alert("a");
            return null;
        }
}
    ).catch(error => {
        if (error.response.status === 400) {
            alert("Bad Username or Password")
        }
        else {
            console.log(error);
        }

    })

};
export async function logout() {
    localStorage.removeItem("user");
    
};
export async function getAllUsers(token) {
    return await axios.get("https://localhost:7274/api/User/get-all-users",{
        headers: {'Authorization': `bearer ${token}`}})
};
export async function getUserById(token,id) {
    return await axios.get(`https://localhost:7274/api/User/get-user-by-id?id=${id}`,{
        headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}})
};
export async function softDeleteUser(token,id) {};
export async function hardDeleteUser(token,id) {};

export async function editUser(token,{id,roleid,name,email,password,phone,isactive,updatedat,createdby,updatedby,isapproved}) {
    const data = {id: id, roleId: roleid,name: name,email: email, password: password, phone: phone, isActive: isactive, updatedAt: updatedat, createdBy: createdby, updatedBy: updatedby, isApproved: isapproved}
    const test = {id,roleid,name,email,password,phone,isactive,updatedat,createdby,updatedby,isapproved}
    //console.log(data)
     return await axios.put(`https://localhost:7274/api/User/edit-user`, test, {
        headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}});
    }
export async function getUnapprovedUsers(token,pageSize,pageNumber) {
    return await axios.get(`https://localhost:7274/api/User/unapproved-users`,{
        headers : { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}
    })
};
export async function changeClientRole(token,id,role) {};

export async function approveUser(token,id) {
    console.log(id)
    return await axios.put(`https://localhost:7274/api/User/approve-user/${id}`,{
        headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}
    })
};
export async function requestRole(token,id,role) {
    console.log(role)
    return await axios.put(`https://localhost:7274/api/Client/request-role?id=${id}&requestedRole=${role}`, 
        {        headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}});
    
};
export async function getAllRoles() {
    return await axios.get("https://localhost:7274/api/User/get-all-roles");
}