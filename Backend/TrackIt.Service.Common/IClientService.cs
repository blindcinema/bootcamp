using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackIt.Models;

namespace TrackIt.Service.Common.Interface
{
    public interface IClientService
    {
        Task<IEnumerable<Package>> GetAllPackagesByClientAsync(Guid clientId);
        Task<Package> TrackPackageAsync(Guid packageId);
        Task<bool> CancelPackageAsync(Guid packageId);
        Task<bool> UpdateDeliveryAsync(Package package);
        Task<bool> AddRatingAsync(Guid clientId, int ratingNumber);
        Task<Rating> AddCommentAsync(Guid ratingId, string comment);
        Task<(bool Success, string ChangedSurname, string ChangedAddress)> UpdateClientAsync(Guid clientId, string surname, string address);
        Task<bool> UpdateUserAsync(Guid userId, string name, string email, string phone, string userName);
        Task RequestRoleAsync(Guid id, Guid requestedRole);

    }
}
