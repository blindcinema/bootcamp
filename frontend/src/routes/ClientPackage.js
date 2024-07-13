import React, { useContext, useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../index.css';
import { getAllPackagesByClient, updatePackage, getPackageInfo } from '../services/packageService'; 
import Navbar from '../components/Navbar';
import { Modal, Button } from 'react-bootstrap';
import { LoginContext } from '../context/LoginContext';
import SortingForm from '../components/SortingForm';

const ClientPackage = () => {
    const loginContext = useContext(LoginContext);
    const [packages, setPackages] = useState([]);
    const [selectedPackage, setSelectedPackage] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [orderBy, setOrderBy] = useState('CreatedAt');
    const [sortOrder, setSortOrder] = useState('Asc');
    const [pageNumber, setPageNumber] = useState(1);
    const [hasMorePackages, setHasMorePackages] = useState(true);
    const pageSize = 3;
    const [newAddress, setNewAddress] = useState('');
    const [newRemark, setNewRemark] = useState('');
    const [isEditMode, setIsEditMode] = useState(false);




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

    const fetchPackages = async () => {
        try {
            const clientId = JSON.parse(localStorage.getItem("user")).roleData; // Replace with actual client ID or fetch dynamically
            const packages = await getAllPackagesByClient(clientId,sortOrder,pageSize,pageNumber);
            console.log(packages)
            //array filter packages which have the deliverylog as "Delivered"
            const filteredPkg = packages.packages.filter((pkg) => {return pkg.deliveryLog === "In Transit"})
            setPackages(filteredPkg);
            setHasMorePackages(filteredPkg.length === pageSize);
        } catch (error) {
            console.error('Error fetching packages', error);
        }
    };
    useEffect(() => {
        

        fetchPackages();
    }, [sortOrder,orderBy, pageNumber]);

    const handleMoreInfo = async (packageId) => {
        try {
            const packageInfo = await getPackageInfo(packageId);
            setSelectedPackage(packageInfo);
            setIsEditMode(false);
            setShowModal(true);
        } catch (error) {
            console.error('Error fetching package info', error);
        }
    };

    const handleEdit = async (packageId) => {
        try {
            const packageInfo = await getPackageInfo(packageId);
            setSelectedPackage(packageInfo);
            setNewAddress(packageInfo.deliveryAddress); 
            setNewRemark(packageInfo.remark);
            setIsEditMode(true);
            setShowModal(true);
        } catch (error) {
            console.error('Error fetching package info', error);
        }
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setSelectedPackage(null);
        setNewAddress('');
        setNewRemark('');
        setIsEditMode(false);
    };

    const handleSortOrderChange = () => {
        
        setSortOrder(prevOrder => (prevOrder === 'Asc' ? 'Desc' : 'Asc'));
        
    };

    const handleUpdatePackage = async () => {
        try {
            await updatePackage(selectedPackage.id, newAddress, newRemark);
            setPackages(packages.map(pkg =>
                pkg.id === selectedPackage.id ? { ...pkg, deliveryAddress: newAddress, remark: newRemark } : pkg
            ));
            handleCloseModal(); 
        } catch (error) {
            console.error('Error updating package', error);
        }
    };

    return (
        <>
            <Navbar />
            <div>
                <div className="container bootstrap-text">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <img width="48" height="48" src="https://img.icons8.com/external-tanah-basah-glyph-tanah-basah/48/external-user-date-and-time-tanah-basah-glyph-tanah-basah.png" alt="user-icon" />
                        <h1>Client</h1>
                        <div></div>
                    </div>
                    
                    <h2>Package Tracking</h2>
                    <div className='flex justify-end '><SortingForm onChange={handleSortOrderChange} /></div>
                    
                    {packages.map(pkg => (
                        <div key={pkg.id} className="package-info">
                            <img width="96" height="96" src="https://img.icons8.com/emoji/96/package-.png" alt="package-" className="package-icon" />
                            <div className="package-tracking">
                                <h5>{pkg.remark}</h5>
                                <p>Tracking number: {pkg.trackingNumber}<br />
                                    <button className="btn btn-primary" onClick={() => handleMoreInfo(pkg.id)}>More package info</button>
                                </p>
                            </div>
                            <div className="edit-button">
                                <button className="btn btn-light" onClick={() => handleEdit(pkg.id)}>Edit</button>
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
                <Modal show={showModal} onHide={handleCloseModal}>
                    <Modal.Header closeButton>
                        <Modal.Title>{isEditMode ? "Edit Package" : "Package Details"}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                       
                        <p><strong>Weight:</strong> {selectedPackage.weight}</p>
                        {isEditMode ? (
                            <>
                                <label>Delivery Address:</label>
                                <input
                                    type="text"
                                    className="form-control mb-3"
                                    value={newAddress}
                                    onChange={(e) => setNewAddress(e.target.value)}
                                />
                                <label>Remark:</label>
                                <textarea
                                    className="form-control"
                                    value={newRemark}
                                    onChange={(e) => setNewRemark(e.target.value)}
                                />
                            </>
                        ) : (
                            <>
                                <p><strong>Remark:</strong> {selectedPackage.remark}</p>
                                <p><strong>Delivery Address:</strong> {selectedPackage.deliveryAddress}</p>
                            </>
                        )}
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCloseModal}>
                            Close
                        </Button>
                        {isEditMode && (
                            <Button variant="primary" onClick={handleUpdatePackage}>
                                Update Package
                            </Button>
                        )}
                    </Modal.Footer>
                </Modal>
            )}
        </>
    );
};

export default ClientPackage;
