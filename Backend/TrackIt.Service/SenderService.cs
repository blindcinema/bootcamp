using System.Threading.Tasks;
using System;
using TrackIt.Models;
using System.Collections.Generic;

public class SenderService : ISenderService
{
    private readonly ISenderRepository _senderRepository;

    public SenderService(ISenderRepository senderRepository)
    {
        _senderRepository = senderRepository;
    }

    public async Task<bool> CreatePackageAsync(Guid senderId, float weight, string remark, string deliveryAddress)
    {
        var package = new Package
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            Weight = weight,
            Remark = remark,
            DeliveryAddress = deliveryAddress,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        return await _senderRepository.CreatePackageAsync(package);
    }
    public async Task<string> GetPackageStatusAsync(Guid packageId)
    {
        return await _senderRepository.GetPackageStatusAsync(packageId);
    }
}
