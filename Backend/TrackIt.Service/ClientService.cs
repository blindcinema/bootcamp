using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackIt.Models;
using TrackIt.Repositories;
using TrackIt.Service.Common.Interface;

namespace TrackIt.Service.Common
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Package>> GetAllPackagesByClientAsync(Guid clientId)
        {
            return await _clientRepository.GetAllPackagesByClientAsync(clientId);
        }

        public async Task<Package> TrackPackageAsync(Guid packageId)
        {
            return await _clientRepository.GetPackageByIdAsync(packageId);
        }

        public async Task<bool> CancelPackageAsync(Guid packageId)
        {
            return await _clientRepository.CancelPackageAsync(packageId);
        }
        public async Task<bool> UpdateDeliveryAsync(Package package)
        {
            return await _clientRepository.UpdatePackageDeliveryAddressAsync(package.Id, package.DeliveryAddress);
        }

        public async Task<bool> AddRatingAsync(Guid clientId, int ratingNumber)
        {
            return await _clientRepository.AddRatingAsync(clientId, ratingNumber);
        }

        public async Task<Rating> AddCommentAsync(Guid ratingId, string comment)
        {
            return await _clientRepository.AddCommentAsync(ratingId, comment);
        }
        public async Task<(bool Success, string ChangedSurname, string ChangedAddress)> UpdateClientAsync(Guid clientId, string surname, string address)
        {
            return await _clientRepository.UpdateClientAsync(clientId, surname, address);
        }

        public async Task<bool> UpdateUserAsync(Guid userId, string name, string email, string phone, string userName)
        {
            return await _clientRepository.UpdateUserAsync(userId, name, email, phone, userName);
        } 
        public async Task RequestRoleAsync(Guid id, Guid requestedRole)
        {
             await _clientRepository.RequestRoleAsync(id, requestedRole);
        }
    }
}
