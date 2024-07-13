import React, { useEffect, useState } from 'react';
import Navbar from "../components/Navbar";
import { getAvailablePackages, markPackageStatus, getPackageInfo } from '../services/packageService';
import SortingForm from '../components/SortingForm';
import { Modal, Button } from 'react-bootstrap';

function CourierPackagesPending() {
    const [packages, setPackages] = useState([]);
    const [sortOrder, setSortOrder] = useState('Asc');
    const [orderBy, setOrderBy] = useState('CreatedAt');
    const [pageNumber, setPageNumber] = useState(1);
    const [hasMorePackages, setHasMorePackages] = useState(true);
    const [selectedPackage, setSelectedPackage] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const pageSize = 3;

    const fetchPackages = async () => {
        try {
            const result = await getAvailablePackages(sortOrder, orderBy, pageSize, pageNumber);
            const fetchedPackages = result.packages || [];
            setPackages(fetchedPackages);
            setHasMorePackages(fetchedPackages.length === pageSize);
        } catch (error) {
            console.error('Error fetching packages', error);
            setPackages([]);
            setHasMorePackages(false);
        }
    };

    useEffect(() => {
        fetchPackages();
    }, [sortOrder, orderBy, pageNumber, pageSize]);

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

    const handleSortingChange = (selectedSortOrder) => {
        setSortOrder(selectedSortOrder);
    };
    const handleSortOrderChange = () => {
        setSortOrder(prevOrder => (prevOrder === 'Asc' ? 'Desc' : 'Asc'));
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

    const handleCloseModal = () => {
        setShowModal(false);
        setSelectedPackage(null);
    };

    const handleMarkInTransit = async (packageId) => {
        try {
            await markPackageStatus(packageId, 'In Transit');
            fetchPackages();
        } catch (error) {
            console.error('Error marking package status', error);
            alert('An error occurred while updating package status');
        }
    };

    return (
        <div>
            <Navbar />
            <div className="container">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <img width="48" height="48" src="https://img.icons8.com/external-tanah-basah-glyph-tanah-basah/48/external-user-date-and-time-tanah-basah-glyph-tanah-basah.png" alt="external-user-date-and-time-tanah-basah-glyph-tanah-basah" />
                </div>
                <h2 className='text-3xl'>Package Tracking</h2>
                <div className="d-flex justify-end align-items-center mb-3">
                    <SortingForm onChange={handleSortOrderChange} />
                </div>
                
                {packages.length > 0 ? (
                    packages.map(pkg => (
                        <div key={pkg.id} className="package-info mb-3">
                            <img width="96" height="96" src="https://img.icons8.com/emoji/96/package-.png" alt="package-" className="package-icon" />
                            <div className="package-tracking">
                                <h5 className='text-black'>{pkg.remark} </h5>
                                <p className='text-black'>Tracking number: {pkg.trackingNumber}<br />
                                <button className="btn btn-primary" onClick={() => handleMoreInfo(pkg.id)}>More package info</button>
                                </p>
                            </div>
                            <div className="delivery-button">
                                <button className="btn btn-light" onClick={() => handleMarkInTransit(pkg.id)}>
                                    Mark as in transit
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p>No packages available.</p>
                )}
                
                <div className="d-flex justify-content-between mt-4">
                    <button className="btn btn-primary" onClick={handlePreviousPage} disabled={pageNumber === 1}>Previous</button>
                    <button className="btn btn-primary" onClick={handleNextPage} disabled={!hasMorePackages}>Next</button>
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
        </div>
    );
}

export default CourierPackagesPending;
