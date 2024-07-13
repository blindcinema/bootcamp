using System;
using System.Collections.Generic;
using System.Linq;
using TrackIt.Models;
using System.Threading.Tasks;
using TrackIt.Common;

namespace TrackIt.Repository.Common
{
    public interface ICourierRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync();
        Task<bool> MarkPaymentAsPaidAsync( Guid paymentId);
    }
}
