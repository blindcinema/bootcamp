import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../index.css';
import { getAllPackagesBySender, cancelPackage, getPackageInfo } from '../services/packageService';
import { useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar';
import { Modal, Button } from 'react-bootstrap';

import SortingForm from '../components/SortingForm';

const SenderPackage = () => {
    const [packages, setPackages] = useState([]);
    const [sortOrder, setSortOrder] = useState('Asc');
    const [orderBy, setOrderBy] = useState('CreatedAt');
    const [pageNumber, setPageNumber] = useState(1);
    const [hasMorePackages, setHasMorePackages] = useState(true);
    const pageSize = 3;

    const [selectedPackage, setSelectedPackage] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const navigate = useNavigate();


    const handleNextPage = () => {
        if (hasMorePackages) {
            setPageNumber(prevPage => prevPage + 1);
        }
    };

    const handlePreviousPage = () => {
        if (pageNumber > 1) {
            setPageNumber(prevPage => prevPage - 1);
        }
    };
    const handleSortOrderChange = () => {
        setSortOrder(prevOrder => (prevOrder === 'Asc' ? 'Desc' : 'Asc'));
    };
    const fetchPackages = async () => {
        try {
            const senderId = JSON.parse(localStorage.getItem("user")).roleData; // Replace with actual sender ID
            const packages = await getAllPackagesBySender(senderId,sortOrder,pageSize,pageNumber);
            console.log(packages)
            setPackages(packages.packages);
            setHasMorePackages(packages.packages.length === pageSize);
        } catch (error) {
            console.error('Error fetching packages', error);
        }
    };
    useEffect(() => {


        fetchPackages();
        
    }, [sortOrder, orderBy, pageNumber]);

    const handleCancel = async (packageId) => {
        try {
            await cancelPackage(packageId);
            setPackages(packages.filter(pkg => pkg.id !== packageId));
        } catch (error) {
            console.error('Error cancelling package', error);
        }
    };

    const handleMoreInfo = async (packageId) => {
        try {
            const packageInfo = await getPackageInfo(packageId);
            setSelectedPackage(packageInfo);
            setShowModal(true);
        } catch (error) {
            console.error('Error fetching package info', error);
        }
    };

    const handleCloseModal = () => setShowModal(false);

    return (
        <>
            <Navbar />
            <div>
                <div className="container bootstrap-text">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <img width="48" height="48" src="https://img.icons8.com/external-tanah-basah-glyph-tanah-basah/48/external-user-date-and-time-tanah-basah-glyph-tanah-basah.png" alt="external-user-date-and-time-tanah-basah-glyph-tanah-basah" />
                        <h1>Sender</h1>
                        <div></div>
                    </div>
                    <h2>Package Tracking</h2>
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <button className="btn btn-success" onClick={() => navigate('/createPackage')}>Create new</button>
                        <SortingForm onChange={handleSortOrderChange}></SortingForm>
                    </div>
                    {packages.map(pkg => (
                        <div key={pkg.id} className="package-info">
                            <img width="96" height="96" src="https://img.icons8.com/emoji/96/package-.png" alt="package-" className="package-icon" />
                            <div className="package-tracking text-black">
                                <h5>{pkg.remark}</h5>
                                <p className='text-black'>Tracking number: {pkg.trackingNumber}<br />
                                    <button className="btn btn-primary" onClick={() => handleMoreInfo(pkg.id)}>More package info</button>
                                </p>
                            </div>
                            <div className="delivery-button">
                                <button className="btn btn-danger" onClick={() => handleCancel(pkg.id)}>Cancel package</button>
                            </div>
                        </div>
                    ))}
                    <div className="d-flex justify-content-between mt-4">
                    <button className="btn btn-primary" onClick={handlePreviousPage} disabled={pageNumber === 1}>Previous</button>
                    <button className="btn btn-primary" onClick={handleNextPage} disabled={!hasMorePackages}>Next</button>
                </div>
                </div>
            </div>

            {selectedPackage && (
                <Modal show={showModal} onHide={handleCloseModal} className='text-black'>
                    <Modal.Header closeButton>
                        <Modal.Title>Package Details</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        
                        <p><strong>Weight:</strong> {selectedPackage.weight}</p>
                        <p><strong>Remark:</strong> {selectedPackage.remark}</p>
                        <p><strong>Delivery Address:</strong> {selectedPackage.deliveryAddress}</p>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCloseModal}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>
            )}
        </>
    );
};

export default SenderPackage;
