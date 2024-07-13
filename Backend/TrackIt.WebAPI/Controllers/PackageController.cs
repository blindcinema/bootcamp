
using Microsoft.AspNetCore.Mvc;
using TrackIt.Models;
using TrackIt.Service.Common;
using TrackIt.Common;
using Microsoft.AspNetCore.Authorization;
using TrackIt.Service.Common.Interface;
using TrackIt.WebAPI.Model;
using TrackIt.Service;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrackIt.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            this._packageService = packageService;
        }

        [HttpGet("get-all-clients")]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients()
        {
            var clients = await _packageService.GetAllClientsAsync();

            var clientDtos = clients.Select(c => new ClientDto
            {
                ClientId = c.Id,
                Surname = c.Surname
            });

            return Ok(clientDtos);
        }


        [HttpGet("get-all-couriers")]
        public async Task<ActionResult<IEnumerable<CourierDto>>> GetAllCouriers()
        {
            try
            {
                var couriers = await _packageService.GetAllCouriersAsync();
                var courierDtos = couriers.Select(c => new CourierDto
                {
                    CourierId = c.Id,
                    Surname = c.Surname
                });

                return Ok(courierDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Client, Admin")]
        [HttpGet("packages-by-client/{clientId}")]
        public async Task<ActionResult<List<Package>>> GetAllPackagesByClient(Guid clientId, string sortOrder = "Asc", string orderBy = "CreatedAt",
                int pageSize = 3, int pageNumber = 1)
        {
            Paging availablePackages = new Paging
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Sorting sorting = new Sorting(orderBy, sortOrder);

            var packages = await _packageService.GetAllPackagesByClientAsync(clientId, sorting, availablePackages);
            return Ok(packages);
        }
        [Authorize(Roles = "Sender, Admin")]
        [HttpGet("packages-by-sender/{senderId}")]
        public async Task<ActionResult<List<Package>>> GetAllPackagesBySender(Guid senderId, string sortOrder = "Asc", string orderBy = "CreatedAt",
                int pageSize = 3, int pageNumber = 1)
        {
            Paging availablePackages = new Paging
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Sorting sorting = new Sorting(orderBy, sortOrder);

            var packages = await _packageService.GetAllPackagesBySenderAsync(senderId, sorting, availablePackages);
            return Ok(packages);
        }

        [Authorize(Roles = "Sender, Admin")]
        [HttpPost("create-package")]
        public async Task<ActionResult> CreatePackageAsync([FromBody] CreatePackageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var defaultCourierId = Guid.Empty; // Default CourierId
                var courierId = request.CourierId ?? defaultCourierId;

                var result = await _packageService.CreatePackageAsync(request.SenderId, courierId, request.Weight, request.Remark, request.DeliveryAddress, request.ClientId, request.CreatedBy, request.UpdatedBy);
                if (result)
                    return Ok();
                else
                    return BadRequest("Failed to create package.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Client, Admin")]
        [HttpPut("cancel/{packageId}/")]
        public async Task<ActionResult> CancelPackage(Guid packageId)
        {
            var result = await _packageService.CancelPackageAsync(packageId);
            if (!result) return NotFound();
            return NoContent();
        }

        //[Authorize(Roles = "Client, Admin")]
        [HttpPut("update-package/{packageId}")]
        public async Task<ActionResult> UpdatePackage(Guid packageId, [FromBody] UpdateAddressRequest request)
        {
            var result = await _packageService.UpdatePackageAsync(packageId, request.NewAddress, request.NewRemark);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Courier, Sender, Admin")]
        [HttpGet("get-available-packages")]
        public async Task<ActionResult<List<Package>>> GetAvailablePackagesAsync(string sortOrder = "Asc", string orderBy = "CreatedAt",
                int pageSize = 3, int pageNumber = 1)
        {
            
            try
            {
                Paging availablePackages = new Paging
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
       
                Sorting sorting = new Sorting(orderBy, sortOrder);

                availablePackages = await _packageService.GetAvailablePackagesAsync(sorting, availablePackages);
                if (availablePackages.Packages.Count == 0)
                {
                    return NoContent();
                }
                return Ok(availablePackages);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Courier")]
        [HttpGet("get-intransit-packages")]
        public async Task<ActionResult<List<Package>>> GetInTransitPackagesAsync(string sortOrder = "Asc", string orderBy = "CreatedAt",
                int pageSize = 3, int pageNumber = 1)
        {

            try
            {
                Paging availablePackages = new Paging
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                Sorting sorting = new Sorting(orderBy, sortOrder);

                availablePackages = await _packageService.GetInTransitPackagesAsync(sorting, availablePackages);
                if (availablePackages.Packages.Count == 0)
                {
                    return NoContent();
                }
                return Ok(availablePackages);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }











        [Authorize(Roles = "Courier, Client, Admin")]
        [HttpGet("get-package-info/{packageId}")]
        public async Task<ActionResult<Package>> GetPackageInfoAsync(Guid packageId)
        {
            try
            {
                var package = await _packageService.GetPackageInfoAsync(packageId);
                if (package == null)
                {

                    return NotFound();
                }

                return Ok(package);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add-rating-and-comment")]
        public async Task<ActionResult> AddRatingAndCommentAsync([FromBody] AddRatingAndCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _packageService.AddRatingAndCommentAsync(request.ClientId,request.PackageId, request.RatingNumber, request.Comment);
                if (result)
                    return Ok();
                else
                    return BadRequest("Failed to add rating and comment.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Courier, Admin")]
        [HttpPut("mark-package-status/{packageId}")]
        public async Task<IActionResult> MarkPackageStatusAsync(Guid packageId, string status)
        {   if (String.IsNullOrEmpty(status))
            {
                return BadRequest();
            }
            else
            {    
                    bool updated = await _packageService.MarkPackageStatusAsync(packageId, status);
                    if (!updated)
                    {
                        return NotFound();
                    }
                    return Ok();
                
            }
        }
        //[Authorize(Roles = "Courier, Admin")]
        //[HttpPut("mark-as-in-transit/{packageId}")]
        //public async Task<IActionResult> MarkAsInTransitAsync(Guid packageId)
        //{
        //    bool updated = await _packageService.MarkAsInTransitAsync(packageId);
        //    if (!updated)
        //    {
        //        return NotFound();
        //    }
        //    return Ok();
        //}




    }
}
