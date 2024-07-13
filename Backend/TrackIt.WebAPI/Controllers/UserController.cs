using Microsoft.AspNetCore.Mvc;
using TrackIt.Service;
using TrackIt.Models;
using AutoMapper;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using TrackIt.Service.Common;
using TrackIt.Common;

namespace TrackIt.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private IUserService _userService;
        public static User user = new User();

        public UserController(IConfiguration configuration, Service.IUserService manageUserService, IMapper mapper)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userService = manageUserService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(CreateUserDto request)
        {
            var mappedUser = _mapper.Map<UserDto>(request);

            //Console.Write(mappedUser);
            // if name, phone, email, username or password are empty or null throw error
            var user = await _userService.CreateUserAsync(mappedUser);
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginUserDto request)
        {
            if (String.IsNullOrEmpty(request.UserName) || String.IsNullOrEmpty(request.Password)) {

                return BadRequest("Username and password must not be empty");
            }
            var mappedUser = _mapper.Map<UserDto>(request);
            //Console.Write(JsonSerializer.Serialize(mappedUser));
            try
            {
                var result = await _userService.GetUserLoginAsync(mappedUser);
                if (!String.IsNullOrEmpty(result.UserName))
                {
                    user = result;
                    //Console.WriteLine(JsonSerializer.Serialize(user));
                    return Ok(result);
                }
                else { return BadRequest("Username and/or password are incorrect"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userService.GetAllUsersAsync();
            //HttpContext.Response.Headers.Append("token",user.Token);

            //Console.WriteLine("got users");

        }
        [Authorize(Roles ="Admin")]
        [HttpGet("get-user-by-id")]
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _userService.GetUserByIdAsync(id);
            //HttpContext.Response.Headers.Append("token",user.Token);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("hard-delete-user")]
        public async Task<List<User>> HardDeleteUserAsync(Guid id)
        { // TRENUTNO SAMO VRATI LISTU NAZAD
            return await _userService.HardDeleteUserAsync(id);

        }
        [Authorize(Roles ="Admin")]
        [HttpPost("soft-delete-user")]
        public async Task<List<User>> SoftDeleteUserAsync(Guid id)
        { // TRENUTNO SAMO VRATI LISTU NAZAD
            return await _userService.SoftDeleteUserAsync(id);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("edit-user")]
        public async Task<User> EditUserAsync(EditUserDto editUserDto)
        { // treba optional params jer se ne mora sve editat tako da vjv treba premapirat is optional user u userDto
            // TODO: EditUser na service, repository i interfaces

            var mappedUser = _mapper.Map<UserDto>(editUserDto);
            return await _userService.EditUserAsync(mappedUser);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}/update-password")]
        public async Task<IActionResult> UpdatePasswordAsync(Guid id,  string password)
        {
            await _userService.UpdatePasswordAsync(id, password);
            return Ok();
        }
        
        [HttpGet("unapproved-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagingUser>> GetUnapprovedUsersAsync([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var pagedUsers = await _userService.GetUnapprovedUsersAsync(pageSize, pageNumber);
            return Ok(pagedUsers);
        }


        [HttpPut("change-client-role/{clientId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeClientRoleAsync(Guid clientId, [FromBody] string newRole)
        {
            try
            {
                var result = await _userService.ChangeClientRoleAsync(clientId, newRole);
                if (!result)
                {
                    return NotFound($"Client with ID {clientId} not found or unable to change role.");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("approve-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApproveUserAsync(Guid userId)
        {
            try
            {
                var result = await _userService.ApproveUserAsync(userId);
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found or role already approved.");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("get-all-roles")]
        [Authorize(Roles = ("Admin"))]
        public async Task<ActionResult> GetAllRolesAsync()
        {
            try
            {
                var result = await _userService.GetAllRolesAsync();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
