import axios from "axios";

export async function updateClient(id, surname, address, token) {

    
return await axios.put(`https://localhost:7274/api/Client/update-client/${id}`,{surname,address}, {
    headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}
})

.then(response => {
        
    if (response.status === 200) {
        response = response.data;      
        return response;
        //console.log(response);
    }
    else {
        return null;
    }
}).catch(err => {
    console.log(err);
})




};
export async function updateUser({token,id,name,email,phone,username}) {
    return await axios.put(`https://localhost:7274/api/Client/update-user/${id}`,{name,email,phone,username}, {
        headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`}}
    )
.then(response => {
        
    if (response.status === 200) {
        response = response.data;      
        return response;
        //console.log(response);
    }
    else {
        return null;
    }
}).catch(err => {
    console.log(err);
})
};