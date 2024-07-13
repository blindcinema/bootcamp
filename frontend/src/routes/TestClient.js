import React, { useEffect, useState } from "react";
import { updateClient } from "../services/clientService";

export function TestClient() {
    const [result,setResult] = useState("");
    const [form,setForm] = useState({});

    const updateClientAsync = async () => {
        const token = JSON.parse(localStorage.getItem("user")).token
        const id = JSON.parse(localStorage.getItem("user")).roleData;
        const result = updateClient(id, form.surname,form.address, token);
        setResult(result);
    }
    useEffect( () => {
        if (localStorage.getItem("user")) {

        }

        else {
            setResult("");
        }
        },
        []);
    function submitForm(e) {
        e.preventDefault();
        updateClientAsync();

    }
    
    function handleChange(e) {
        const name = e.target.name;
        const value = e.target.value;
        setForm(values => ({...values, [name]: value}))
        
    }
    return (

        <div className="flex justify-center items-center mt-24 flex-col gap-4">
            <form onSubmit={submitForm} className="flex flex-col">
                <label htmlFor="surname" >Surname</label>
                <input name="surname" id="surname" type="text" onChange={handleChange}></input>
                <label htmlFor="address" >Address</label>
                <input name="address" id="address" type="text" onChange={handleChange}></input>
                <button type="submit">Submit</button>
            </form>
            {result.value}
            </div>
    );
}