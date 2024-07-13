using System;
using System.Collections.Generic;
using TrackIt.Common;
using TrackIt.Models;
using System.Threading.Tasks;

namespace TrackIt.Service.Common
{
    public interface ICourierService
    {
        Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync();
        Task<bool> MarkPaymentAsPaidAsync(Guid paymentId);

    }
}
