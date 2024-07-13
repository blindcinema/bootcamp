
using Microsoft.AspNetCore.Mvc;
using TrackIt.Models;
using TrackIt.Service.Common;
using TrackIt.Common;
using Microsoft.AspNetCore.Authorization;

namespace TrackIt.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            this._courierService = courierService;
        }


        [HttpGet("paymentMethods")]
        [Authorize(Roles = ("Courier"))]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetAllPaymentMethods()
        {
            var paymentMethods = await _courierService.GetAllPaymentMethodsAsync();
            return Ok(paymentMethods);
        }

        [HttpPut("packages/{paymentId}/markAsPaid")]
        [Authorize(Roles = ("Courier"))]
        public async Task<IActionResult> MarkPaymentAsPaid( Guid paymentId)
        {
            var result = await _courierService.MarkPaymentAsPaidAsync(paymentId);
            if (!result) return NotFound();
            return NoContent();
        }




    }
}
