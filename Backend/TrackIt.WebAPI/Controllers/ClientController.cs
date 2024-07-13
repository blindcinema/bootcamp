using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackIt.Models;
using TrackIt.Service.Common.Interface;
using TrackIt.WebAPI.Model;

namespace TrackIt.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // Pregled svih paketa
        //[HttpGet("{clientId}/packages")]
        //public async Task<ActionResult<IEnumerable<Package>>> GetAllPackagesByClient(Guid clientId)
        //{
        //    var packages = await _clientService.GetAllPackagesByClientAsync(clientId);
        //    return Ok(packages);
        //}

        // Praćenje paketa
        //[HttpGet("package/{packageId}")]
        //public async Task<ActionResult<Package>> GetPackageById(Guid packageId)
        //{
        //    var package = await _clientService.TrackPackageAsync(packageId);
        //    if (package == null) return NotFound();
        //    return Ok(package);
        //}

        //// Soft delete a package
        ////[HttpPut("package/{packageId}/cancel")]
        ////public async Task<ActionResult> CancelPackage(Guid packageId)
        ////{
        ////    var result = await _clientService.CancelPackageAsync(packageId);
        ////    if (!result) return NotFound();
        ////    return NoContent();
        ////}

        //// Ažuriranje adrese dostave
        //[HttpPut("package/{packageId}/address")]
        //public async Task<ActionResult> UpdatePackageDeliveryAddress(Guid packageId, [FromBody] string newAddress)
        //{
        //    var package = new Package { Id = packageId, DeliveryAddress = newAddress };
        //    var result = await _clientService.UpdateDeliveryAsync(package);
        //    if (!result) return NotFound();
        //    return NoContent();
        //}

        //// Dodavanje ocene
        //[HttpPost("rating")]
        //public async Task<ActionResult> AddRating(Guid clientId, [FromBody] int ratingNumber)
        //{
        //    var result = await _clientService.AddRatingAsync(clientId, ratingNumber);
        //    if (!result) return BadRequest();
        //    return NoContent();
        //}

        //// Dodavanje komentara
        //[HttpPut("rating/{ratingId}/comment")]
        //public async Task<ActionResult<Rating>> AddComment(Guid ratingId, [FromBody] string comment)
        //{
        //    var updatedRating = await _clientService.AddCommentAsync(ratingId, comment);
        //    if (updatedRating == null) return NotFound();
        //    return Ok(updatedRating);
        //}

        // Ažuriranje klijenta
        [Authorize(Roles ="Client")]
        [HttpPut("update-client/{clientId}")]
        public async Task<ActionResult> UpdateClient(Guid clientId, [FromBody] UpdateClientRequest request)
        {
            var (success, changedSurname, changedAddress) = await _clientService.UpdateClientAsync(clientId, request.Surname, request.Address);
            if (!success) return NotFound();

            var response = new
            {
                ChangedSurname = changedSurname,
                ChangedAddress = changedAddress
            };

            return Ok(response);
        }

        // Ažuriranje korisnika

        [HttpPut("update-user/{userId}")]
        public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var result = await _clientService.UpdateUserAsync(userId, request.Name, request.Email, request.Phone, request.UserName);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("request-role")]
        [Authorize(Roles =("Client"))]
        public async Task<ActionResult> RequestRoleAsync(Guid id, Guid requestedRole)
        {
            try
            {
                await _clientService.RequestRoleAsync(id, requestedRole);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    
}
