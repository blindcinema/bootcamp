import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../index.css';
import Navbar from '../components/Navbar';
import { Modal, Button, Form } from 'react-bootstrap';
import { getAllPackagesByClient, getPackageInfo, addRatingAndComment } from '../services/packageService';
import SortingForm from '../components/SortingForm';

const ClientPackage = () => {
    const [packages, setPackages] = useState([]);
    const [sortOrder, setSortOrder] = useState('Asc');
    const [selectedPackage, setSelectedPackage] = useState(null);
    const [pageNumber, setPageNumber] = useState(1);
    const [orderBy, setOrderBy] = useState('CreatedAt');
    const [showModal, setShowModal] = useState(false);
    const [rating, setRating] = useState(1);
    const [comment, setComment] = useState('');
    const [hasMorePackages, setHasMorePackages] = useState(true);
    const [showRatingComment, setShowRatingComment] = useState(false); // State to manage showing rating and comment inputs
    const pageSize = 3;


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
            const filteredPkg = packages.packages.filter((pkg) => {return pkg.deliveryLog === "Delivered"})
            setPackages(filteredPkg);
            setHasMorePackages(filteredPkg.length === pageSize);
        } catch (error) {
            console.error('Error fetching packages', error);
        }
    };

    useEffect(() => {
        
        fetchPackages();
        return () => {
            
        }
    }, [sortOrder,orderBy, pageNumber]);
    
    const handleSortOrderChange = () => {
        
        setSortOrder(prevOrder => (prevOrder === 'Asc' ? 'Desc' : 'Asc'));
        
    };
    const handleMoreInfo = async (packageId) => {
        try {
            const packageInfo = await getPackageInfo(packageId);
            setSelectedPackage(packageInfo);
            setShowModal(true);
            setShowRatingComment(false);
        } catch (error) {
            console.error('Error fetching package info', error);
        }
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setSelectedPackage(null); 
    };

    const handleShowRatingComment = (pkg) => {
        setSelectedPackage(pkg);
        setShowModal(true);
        setShowRatingComment(true); 
    };

    const handleAddRatingAndComment = async () => {
        try {
            const result = await addRatingAndComment(selectedPackage.clientId, selectedPackage.id, rating, comment);
            setShowModal(false);
        } catch (error) {
            console.error('Error adding rating and comment', error);
        }
    };

    return (
        <>
            <Navbar />
            <div className="container bootstrap-text">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <img
                        width="48"
                        height="48"
                        src="https://img.icons8.com/external-tanah-basah-glyph-tanah-basah/48/external-user-date-and-time-tanah-basah-glyph-tanah-basah.png"
                        alt="external-user-date-and-time-tanah-basah-glyph-tanah-basah"
                    />
                    <h1>Client</h1>
                    <div><SortingForm onChange={handleSortOrderChange} /></div>
                </div>
                <h2>Package Tracking</h2>
                {packages.map((pkg) => (
                    <div key={pkg.id} className="package-info">
                        <img
                            width="96"
                            height="96"
                            src="https://img.icons8.com/emoji/96/package-.png"
                            alt="package-"
                            className="package-icon"
                        />
                        <div className="package-tracking">
                            <h5>{pkg.remark}</h5>
                            <p>
                                Tracking number: {pkg.trackingNumber}
                                <br />
                                <button
                                    className="btn btn-primary"
                                    onClick={() => handleMoreInfo(pkg.id)}
                                >
                                    More package info
                                </button>
                            </p>
                        </div>
                                <button
                                    className="btn btn-light"
                                    onClick={() => handleShowRatingComment(pkg)}
                                >
                                    Comment & Rate
                                </button>
                    </div>
                ))}
            </div>
            <div className="d-flex justify-content-between mt-4">
                    <button className="btn btn-primary" onClick={handlePreviousPage} disabled={pageNumber === 1}>Previous</button>
                    <button className="btn btn-primary" onClick={handleNextPage} disabled={!hasMorePackages}>Next</button>
                </div>

            {selectedPackage && (
                <Modal show={showModal} onHide={handleCloseModal}>
                    <Modal.Header closeButton>
                        <Modal.Title>Package Details</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                       
                        <p>
                            <strong>Weight:</strong> {selectedPackage.weight}
                        </p>
                        <p>
                            <strong>Remark:</strong> {selectedPackage.remark}
                        </p>
                        <p>
                            <strong>Delivery Address:</strong>{' '}
                            {selectedPackage.deliveryAddress}
                        </p>
                        {showRatingComment && (
                            <>
                                <Form.Group controlId="rating">
                                    <Form.Label>Rating:</Form.Label>
                                    <Form.Control
                                        type="number"
                                        value={rating }
                                        defaultValue={'1'}
                                        onChange={(e) =>
                                            setRating(parseInt(e.target.value))
                                        }
                                    />
                                </Form.Group>
                                <Form.Group controlId="comment">
                                    <Form.Label>Comment:</Form.Label>
                                    <Form.Control
                                        as="textarea"
                                        rows={3}
                                        value={comment}
                                        onChange={(e) =>
                                            setComment(e.target.value)
                                        }
                                    />
                                </Form.Group>
                            </>
                        )}
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCloseModal}>
                            Close
                        </Button>
                        {showRatingComment && (
                            <Button
                                variant="primary"
                                onClick={handleAddRatingAndComment}
                            >
                                Add Rating & Comment
                            </Button>
                        )}
                    </Modal.Footer>
                </Modal>
            )}
        </>
    );
};

export default ClientPackage;
