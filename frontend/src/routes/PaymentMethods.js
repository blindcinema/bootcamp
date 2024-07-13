import React, { useEffect, useState } from 'react';

import { getAllPaymentMethods } from '../services/courierService';
import MarkPaymentAsPaid from './MarkPaymentAsPaid';
import './PaymentMethod.css';


const PaymentMethods = () => {
    const [paymentMethods, setPaymentMethods] = useState([]);
   
    useEffect(() => {
        const  fetchPaymentMethods = async () => {
            try{
                const data = await getAllPaymentMethods();
                console.log('Fetched payment methods:', data); 
                setPaymentMethods(data);

            }catch(error){
                console.log('Error fetching payment methods:', error);
            }
        };

        fetchPaymentMethods();

    }, []);


    


    return (
        <div className="payment-methods-container">
            <h2>Payment Methods</h2>
            <ul className="payment-methods-list">
            {paymentMethods.map(pm => (
                    <li key={pm.id} className="payment-method-item">
                        <p><strong>Type:</strong> {pm.type}</p>
                        <p><strong>Is Active:</strong> {pm.isActive.toString()}</p>
                        <p><strong>Created At:</strong> {pm.createdAt}</p>
                        <p><strong>Updated At:</strong> {pm.updatedAt}</p>
                        <MarkPaymentAsPaid paymentId={pm.paymentId}  />
                    </li>
                ))}

            </ul>

        </div>
    );

};


export default PaymentMethods;