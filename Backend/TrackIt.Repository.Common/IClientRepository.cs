﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackIt.Models;

namespace TrackIt.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Package>> GetAllPackagesByClientAsync(Guid clientId);
        Task<Package> GetPackageByIdAsync(Guid packageId);
        Task<bool> CancelPackageAsync(Guid packageId);
        Task<bool> UpdatePackageDeliveryAddressAsync(Guid packageId, string newAddress);
        Task<bool> AddRatingAsync(Guid clientId, int ratingNumber);
        Task<Rating> AddCommentAsync(Guid ratingId, string comment);
        Task<(bool Success, string ChangedSurname, string ChangedAddress)> UpdateClientAsync(Guid clientId, string surname, string address);
        Task<bool> UpdateUserAsync(Guid userId, string name, string email, string phone, string userName);
        Task RequestRoleAsync(Guid id, Guid requestedRole);

    }
}