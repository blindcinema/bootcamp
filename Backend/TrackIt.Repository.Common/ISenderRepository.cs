using System.Threading.Tasks;
using System;
using TrackIt.Models;

public interface ISenderRepository
{
    Task<bool> CreatePackageAsync(Package package);
    Task<string> GetPackageStatusAsync(Guid packageId);
    
}
