import React from 'react';

function Package({ package: pkg }) {
    return (
        <div className="package-info">
            <img width="96" height="96" src="https://img.icons8.com/emoji/96/package-.png" alt="package-" className="package-icon" />
            <div className="package-tracking">
                <h5>{pkg.name}</h5>
                <p>Tracking number: {pkg.trackingNumber}<br />More package info: {pkg.info}</p>
            </div>
            <div className="comment-rating">
                <div className="row">
                    {Array.from({ length: 4 }).map((_, index) => (
                        <div className="col-sm-1" key={index}>
                            <img width="24" height="24" src="https://img.icons8.com/ios/24/star--v1.png" alt="star--v1" />
                        </div>
                    ))}
                </div>
                <div className="row mt-1">
                    <div className="col-sm-3">
                        <button className="btn btn-light">Comment</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Package;
