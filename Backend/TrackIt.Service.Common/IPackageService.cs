using System;
using System.Collections.Generic;
using TrackIt.Common;
using TrackIt.Models;
using System.Threading.Tasks;

namespace TrackIt.Service.Common
{
    public interface IPackageService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Paging> GetAllPackagesBySenderAsync(Guid senderId, Sorting sorting, Paging availablePackages);
        Task<Paging> GetAllPackagesByClientAsync(Guid clientId, Sorting sorting, Paging availablePackages);
        Task<IEnumerable<Courier>> GetAllCouriersAsync();
        Task<bool> CreatePackageAsync(Guid senderId, Guid courierId, float weight, string remark, string deliveryAddress, Guid clientId, Guid createdBy, Guid updatedBy);
        Task<bool> CancelPackageAsync(Guid packageId);
        Task<bool> UpdatePackageAsync(Guid packageId, string newAddress, string newRemark);
        Task<bool> AddRatingAndCommentAsync(Guid clientId,Guid packageId, int ratingNumber, string comment);
        Task<Rating> AddCommentAsync(Guid ratingId, string comment);
        Task<Paging> GetAvailablePackagesAsync(Sorting sorting, Paging availablePackages);

        Task<Paging> GetInTransitPackagesAsync(Sorting sorting, Paging availablePackages);
        Task<Package> GetPackageInfoAsync(Guid packageId);

        Task<bool> MarkPackageStatusAsync(Guid packageId, string status);

    }
}
