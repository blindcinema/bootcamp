using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TrackIt.Models;
using TrackIt.WebAPI.Model;

namespace TrackIt.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    {
        private readonly ISenderService _senderService;

        public SenderController(ISenderService senderService)
        {
            _senderService = senderService;
        }

        //[HttpPost("createPackage")]
        //public async Task<ActionResult> CreatePackageAsync([FromBody] CreatePackageRequest request)
        //{
        //    var result = await _senderService.CreatePackageAsync(request.SenderId, request.Weight, request.Remark, request.DeliveryAddress);
        //    if (result)
        //        return Ok();
        //    else
        //        return BadRequest();
        //}


        //[HttpGet("package/{packageId}/status")]
        //public async Task<IActionResult> GetPackageStatus(Guid packageId)
        //{
        //    var status = await _senderService.GetPackageStatusAsync(packageId);
        //    if (string.IsNullOrEmpty(status))
        //    {
        //        return NotFound("Package status not found.");
        //    }
        //    return Ok(status);
        //}
    }
}
