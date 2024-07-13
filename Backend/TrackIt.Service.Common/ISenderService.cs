using System.Threading.Tasks;
using System;
using TrackIt.Models;

public interface ISenderService
{
    Task<bool> CreatePackageAsync(Guid senderId, float weight, string remark, string deliveryAddress);
    Task<string> GetPackageStatusAsync(Guid packageId);
}
