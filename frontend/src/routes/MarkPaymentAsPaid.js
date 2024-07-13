import React from 'react';
import { markPaymentAsPaid } from '../services/courierService';
import './PaymentMethod.css';

const MarkPaymentAsPaid = ({ paymentId }) => {
    
    const handleMarkAsPaid = async () => {
        try {
            const success = await markPaymentAsPaid(paymentId);
           
            alert('Payment marked as paid successfully!');
            
        } catch (error) {
            console.error('Error marking payment as paid: ', error);
        }
    };

    return (
        <button className="button" onClick={handleMarkAsPaid}>Mark as Paid</button>
    );
};

export default MarkPaymentAsPaid;
