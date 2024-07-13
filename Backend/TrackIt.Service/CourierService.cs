using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackIt.Common;
using TrackIt.Models;
using TrackIt.Repository.Common;
using TrackIt.Service.Common;

namespace TrackIt.Service
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        public CourierService(ICourierRepository courierRepository)
        {
            this._courierRepository = courierRepository;
        }
        public async Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync()
        {
            return await _courierRepository.GetAllPaymentMethodsAsync();
        }

        public async Task<bool> MarkPaymentAsPaidAsync(Guid paymentId)
        {
            return await _courierRepository.MarkPaymentAsPaidAsync(paymentId);
        }
    }
}
