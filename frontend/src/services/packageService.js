import axios from 'axios';

// Set your base URL here
const BASE_URL = 'https://localhost:7274/api/Package';

// Add an instance with default configuration if needed
const axiosInstance = axios.create({
    baseURL: BASE_URL,
    headers: {
        'Content-Type': 'application/json',
        // Authorization: 'Bearer your_token' // Add if using authorization
    }
});

export const getAllPackagesByClient = async (clientId,sortOrder,pageSize,pageNumber) => {

    
    try {
        const response = await axiosInstance.get(`/packages-by-client/${clientId}?sortOrder=${sortOrder}&orderBy=CreatedAt&pageSize=10&pageNumber=${pageNumber}`);
        console.log(response.data)
        return response.data;
    } catch (error) {
        console.error('Error fetching packages by client', error);
        throw error;
    }
};

export const getAllPackagesBySender = async (senderId,sortOrder,pageSize,pageNumber) => {
    try {
        const response = await axiosInstance.get(`/packages-by-sender/${senderId}?sortOrder=${sortOrder}&orderBy=CreatedAt&pageSize=3&pageNumber=${pageNumber}`);
        return response.data;
        console.log(response.data)
    } catch (error) {
        console.error('Error fetching packages by sender', error);
        throw error;
    }
};


export const createPackage = async (token, {packageData}) => {
    try {
        const response = await axiosInstance.post('/create-package', packageData, {
             headers: { 'Content-type' : 'application/json', 'Accept' : 'text/json', 'Authorization': `bearer ${token}`} 
            });
        return response.data;
    } catch (error) {
        console.error('Error creating package', error);
        throw error;
    }
};


export const getAllClients = async () => {
    try {
        const response = await axiosInstance.get('/get-all-clients');
        return response.data; // Array of ClientDto [{ id, lastName }, ...]
    } catch (error) {
        console.error('Error fetching clients', error);
        throw error;
    }
};

export const getAllCouriers = async () => {
    try {
        const response = await axiosInstance.get('/get-all-couriers');
        return response.data; 
    } catch (error) {
        console.error('Error fetching couriers', error);
        throw error;
    }
};

export const cancelPackage = async (packageId) => {
    try {
        const response = await axiosInstance.put(`/cancel/${packageId}`);
        return response.data;
    } catch (error) {
        console.error('Error cancelling package', error);
        throw error;
    }
};
export const updatePackage = async (packageId, newAddress, newRemark) => {
    try {
        const response = await axiosInstance.put(`/update-package/${packageId}`, {
            newAddress,
            newRemark
        });
        return response.data; // Assuming backend responds with updated package data
    } catch (error) {
        console.error('Error updating package details', error);
        throw error;
    }
};
export const getAvailablePackages = async (sortOrder = 'Asc', orderBy = 'CreatedAt', pageSize = 10, pageNumber = 1) => {
    try {
        const response = await axiosInstance.get('/get-available-packages', {
            params: { sortOrder, orderBy, pageSize, pageNumber }
        });
        return response.data;
    } catch (error) {
        console.error('Error fetching available packages', error);
        throw error;
    }
};

export const getInTransitPackages = async (sortOrder = 'Asc', orderBy = 'CreatedAt', pageSize = 10, pageNumber = 1) => {
    try {
        const response = await axiosInstance.get('/get-intransit-packages', {
            params: { sortOrder, orderBy, pageSize, pageNumber }
        });
        return response.data;
    } catch (error) {
        console.error('Error fetching in transit packages', error);
        throw error;
    }
};

export const getPackageInfo = async (packageId) => {
    try {
        const response = await axiosInstance.get(`/get-package-info/${packageId}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching package info', error);
        throw error;
    }
};
export const addRatingAndComment = async (clientId, packageId, ratingNumber, comment) => {
    try {
        const response = await axiosInstance.post('/add-rating-and-comment', {
            clientId,
            packageId,
            ratingNumber,
            comment
        });
        return response.data; 
    } catch (error) {
        console.error('Error adding rating and comment', error);
        throw error;
    }
};

export const markPackageStatus = async (packageId, status) => {
    try {
        const response = await axiosInstance.put(`/mark-package-status/${packageId}`, null, {
            params: { status }
        });
        return response.data;
    } catch (error) {
        console.error('Error marking package status', error);
        throw error;
    }
};
