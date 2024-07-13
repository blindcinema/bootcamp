using System;
using System.Collections.Generic;
using System.Linq;
using TrackIt.Models;
using System.Threading.Tasks;
using TrackIt.Common;

namespace TrackIt.Repository.Common
{
    public interface IPackageRepository
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<IEnumerable<Courier>> GetAllCouriersAsync();
        Task<Paging> GetAllPackagesBySenderAsync(Guid senderId, Sorting sort, Paging availablePackages);
        Task<Paging> GetAllPackagesByClientAsync(Guid clientId, Sorting sort, Paging availablePackages);
        Task<bool> CreatePackageAsync(Package package);
        Task<bool> CancelPackageAsync(Guid packageId);
        Task<bool> UpdatePackageAsync(Guid packageId, string newAddress, string newRemark);
        Task<bool> AddRatingAndCommentAsync(Guid clientId, Guid packageId, int ratingNumber, string comment);
        Task<Rating> AddCommentAsync(Guid ratingId, string comment);
        Task<Paging> GetAvailablePackagesAsync(Sorting sorting, Paging paging);

        Task<Paging> GetInTransitPackagesAsync(Sorting sorting, Paging paging);
        Task<Package> GetPackageInfoAsync(Guid packageId);

        Task<bool> MarkPackageStatusAsync(Guid packageId, string status);
        Task<bool> MarkAsInTransitAsync(Guid packageId);
    }
}
