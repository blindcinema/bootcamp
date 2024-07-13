using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackIt.Common;
using TrackIt.Models;
using TrackIt.Repository;
using TrackIt.Repository.Common;
using TrackIt.Service.Common;

namespace TrackIt.Service
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IPackageRepository packageRepository)
        {
            this._packageRepository = packageRepository;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _packageRepository.GetAllClientsAsync();
        }

        public async Task<IEnumerable<Courier>> GetAllCouriersAsync() // Add this method
        {
            return await _packageRepository.GetAllCouriersAsync();
        }
        public async Task<bool> CancelPackageAsync(Guid packageId)
        {
            return await _packageRepository.CancelPackageAsync(packageId);
        }

        public async Task<bool> CreatePackageAsync(Guid senderId, Guid courierId, float weight, string remark, string deliveryAddress, Guid clientId, Guid createdBy, Guid updatedBy)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var trackingNumber = new String(stringChars);

            var package = new Package
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                SenderId = senderId,
                CourierId = courierId,
                Weight = weight,
                Remark = remark,
                DeliveryAddress = deliveryAddress,
                CreatedAt = DateTime.UtcNow,
                TrackingNumber = trackingNumber,
                IsActive = true, // Assuming package is active when created
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
            };

            return await _packageRepository.CreatePackageAsync(package);
        }

        public async Task<bool> UpdatePackageAsync(Guid packageId, string newAddress, string newRemark)
        {
            return await _packageRepository.UpdatePackageAsync(packageId, newAddress, newRemark);
        }


        public async Task<Rating> AddCommentAsync(Guid ratingId, string comment)
        {
            return await _packageRepository.AddCommentAsync(ratingId, comment);
        }
        public async Task<Paging> GetAllPackagesByClientAsync(Guid clientId, Sorting sorting, Paging availablePackages)
        {
            return await _packageRepository.GetAllPackagesByClientAsync(clientId, sorting, availablePackages);
        }

        public async Task<Paging> GetAllPackagesBySenderAsync(Guid senderId, Sorting sorting, Paging availablePackages)
        {
            return await _packageRepository.GetAllPackagesBySenderAsync(senderId, sorting, availablePackages);
        }

        public async Task<Paging> GetAvailablePackagesAsync(Sorting sorting, Paging availablePackages)
        {
            return await _packageRepository.GetAvailablePackagesAsync(sorting, availablePackages);
        }

        public async Task<Package> GetPackageInfoAsync(Guid packageId)
        {
            return await _packageRepository.GetPackageInfoAsync(packageId);
        }

        public async Task<bool> AddRatingAndCommentAsync(Guid clientId,Guid packageId, int ratingNumber, string comment)
        {
            return await _packageRepository.AddRatingAndCommentAsync(clientId, packageId, ratingNumber, comment);
        }


        public async Task<bool> MarkPackageStatusAsync(Guid packageId, string status)
        {
            return await _packageRepository.MarkPackageStatusAsync(packageId, status);
        }

        public async Task<Paging> GetInTransitPackagesAsync(Sorting sorting, Paging availablePackages)
        {
            return await _packageRepository.GetInTransitPackagesAsync(sorting, availablePackages);
        }
    }
}
