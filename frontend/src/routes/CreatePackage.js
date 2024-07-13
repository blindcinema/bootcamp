import React, { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import Navbar from '../components/Navbar';
import { createPackage, getAllClients, getAllCouriers } from '../services/packageService'; // Adjust import based on your service file
import { LoginContext } from '../context/LoginContext';

const CreatePackage = () => {
    const loginContext = useContext(LoginContext);
    const [weight, setWeight] = useState('');
    const [deliveryAddress, setDeliveryAddress] = useState('');
    const [remark, setRemark] = useState('');
    const [clients, setClients] = useState([]);
    const [selectedClient, setSelectedClient] = useState('');
    const [couriers, setCouriers] = useState([]);
    const [selectedCourier, setSelectedCourier] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const fetchedClients = await getAllClients();
                setClients(fetchedClients);
                if (fetchedClients.length > 0) {
                    setSelectedClient(fetchedClients[0].clientId); // Set the ID of the first client as default
                }
                
                const fetchedCouriers = await getAllCouriers();
                setCouriers(fetchedCouriers);
                if (fetchedCouriers.length > 0) {
                    setSelectedCourier(fetchedCouriers[0].courierId); // Set the ID of the first courier as default
                }
            } catch (error) {
                console.error('Error fetching data', error);
            }
        };

        fetchData();
    }, []);

    const handleSubmit = async (event) => {
        event.preventDefault();
        const token = JSON.parse(localStorage.getItem("user")).token;
        const packageData = {
            senderId: JSON.parse(localStorage.getItem("user")).roleData, // Replace with actual sender ID
            clientId: selectedClient,
            courierId: selectedCourier, // Include courierId
            weight: parseFloat(weight),
            remark: remark,
            deliveryAddress: deliveryAddress,
            CreatedBy: JSON.parse(localStorage.getItem("user")).id,
            UpdatedBy: JSON.parse(localStorage.getItem("user")).id
        };
        
        console.log('Package Data:', packageData); // Log packageData to check its structure

        try {

            await createPackage(token,{packageData});
            console.log('Package created successfully');
            navigate('/sender');
        } catch (error) {
            console.error('Failed to create package', error);
        }
    };

    return (
        <>
            <Navbar />
            <div className="login-container container">
                <h1>Create Package</h1>
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <input
                            type="number"
                            className="form-control"
                            placeholder="Weight"
                            value={weight}
                            onChange={(e) => setWeight(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <input
                            type="text"
                            className="form-control"
                            placeholder="Delivery Address"
                            value={deliveryAddress}
                            onChange={(e) => setDeliveryAddress(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <input
                            type="text"
                            className="form-control"
                            placeholder="Message"
                            value={remark}
                            onChange={(e) => setRemark(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="recipient">Select Recipient:</label>
                        <select
                            id="recipient"
                            className="form-select"
                            value={selectedClient}
                            onChange={(e) => setSelectedClient(e.target.value)}
                            required
                        >
                            {clients.map(client => (
                                <option key={client.clientId} value={client.clientId}>{client.surname}</option>
                            ))}
                        </select>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="courier">Select Courier:</label>
                        <select
                            id="courier"
                            className="form-select"
                            value={selectedCourier}
                            onChange={(e) => setSelectedCourier(e.target.value)}
                            required
                        >
                            {couriers.map(courier => (
                                <option key={courier.courierId} value={courier.courierId}>{courier.surname}</option>
                            ))}
                        </select>
                    </div>
                    <button type="submit" className="btn btn-primary">Create Package</button>
                </form>
            </div>
        </>
    );
};

export default CreatePackage;
