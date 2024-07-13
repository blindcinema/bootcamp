import axios from 'axios';

const BASE_URL = 'https://localhost:7274/api/Courier';


const axiosInstance = axios.create({
    baseURL: BASE_URL,
    headers: {
        'Content-Type': 'application/json',
        // Authorization: 'Bearer your_token' // Add if using authorization
    }
});

export const  getAllPaymentMethods = async () =>{
    try{
        const response = await axiosInstance.get('/paymentMethods');
        return response.data;
    }catch (error) {
        console.error('Error fetching payment methods: ', error);
        throw error;
    }

};

export const markPaymentAsPaid = async (paymentId) => {
    try {
        const response = await axiosInstance.put(`/packages/${paymentId}/markAsPaid`);
        console.log('Response from markPaymentAsPaid:', response);
        return response.data;
    } catch (error) {
        console.error('Error marking payment as paid: ', error);
        throw error;
    }
};