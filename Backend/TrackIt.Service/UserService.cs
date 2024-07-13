using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using TrackIt.Models;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System;
using AutoMapper;
using TrackIt.Common;
namespace TrackIt.Service
{
    public class UserService : IUserService
    {
        public bool requestValid = true;
        private IConfiguration _configuration;
        private IMapper _mapper;
        private global::IUserRepository _manageUser;
        public static User user = new User();
        public UserService(IConfiguration configuration, global::IUserRepository manageUser, IMapper mapper)
        {
            _mapper = mapper;
            _configuration = configuration;
            _manageUser = manageUser;

        }
        public async Task<User> CreateUserAsync(UserDto request)
        {   
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.UserName = request.UserName;
            user.Password = passwordHash;
            user.Phone = request.Phone;
            user.Email = request.Email;
            user.CreatedBy = request.CreatedBy;
            user.UpdatedBy = request.UpdatedBy;
            user.CreatedAt = request.CreatedAt;
            user.UpdatedAt = request.UpdatedAt;
            user.RoleId = request.Id;
            user.Name = request.Name;
            user.IsActive = request.IsActive;
            user.Id = Guid.NewGuid();
            var result = await _manageUser.CreateUserAsync(user);
            return result;
        }
        public async Task<User> GetUserLoginAsync(UserDto request)
        { // return user object here
            user = _mapper.Map<User>(request);
            //Console.WriteLine(JsonSerializer.Serialize(user));
            var result = await _manageUser.GetUserLoginAsync(user);
            if (request.UserName != user.UserName)
            {
                requestValid = false;
                throw new Exception("I");       
            }
            if (!BCrypt.Net.BCrypt.Verify(user.Password, result.Password))
            {
                requestValid = false;
                throw new Exception("");
            }
            else
            {
                //Console.Write(JsonSerializer.Serialize(result));
                BCrypt.Net.BCrypt.Verify(request.Password, result.Password);
                string token = CreateToken(result);
                result.Token = token;
                return result;  
                //return token;
            }

        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var result = await _manageUser.GetUserByIdAsync(id);
            return result;

        }
        public async Task<List<User>> GetAllUsersAsync()
        {   
            var result = await _manageUser.GetAllUsersAsync();
            return result;

        }


        public async Task<List<User>> SoftDeleteUserAsync(Guid id)
        {
            return await _manageUser.SoftDeleteUserAsync(id);
        }
        public async Task<List<User>> HardDeleteUserAsync(Guid id)
        {
            return await _manageUser.HardDeleteUserAsync(id);
        }

        public async Task<User> EditUserAsync(UserDto request)
        {
            user = _mapper.Map<User>(request);
            var result = await _manageUser.EditUserAsync(user);
            return result;

        }

        public async Task UpdatePasswordAsync(Guid id, string password)
        {

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _manageUser.UpdatePasswordAsync(id, passwordHash);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }

        public async Task<bool> ChangeClientRoleAsync(Guid clientId, string newRole)
        {
            var client = await _manageUser.GetClientByIdAsync(clientId);
            if (client == null) return false;

            var user = await _manageUser.GetUserByIdAsync(client.UserId);
            if (user == null) return false;

            var role = await _manageUser.GetRoleByDescriptionAsync(newRole);
            if (role == null) return false;

            user.RoleId = role.Id;
            user.IsApproved = false;
            await _manageUser.UpdateUserRoleAsync(user);

            if (newRole == "Courier")
            {
                var courier = new Courier
                {
                    Id = Guid.NewGuid(),
                    Surname = client.Surname,
                    UserId = user.Id
                };
                await _manageUser.AddCourierAsync(courier);
                await _manageUser.DeleteClientAsync(clientId);

            }
            else if (newRole == "Sender")
            {
                var sender = new Sender
                {
                    Id = Guid.NewGuid(),
                    Address = client.Address,
                    UserId = user.Id
                };
                await _manageUser.AddSenderAsync(sender);
                await _manageUser.DeleteClientAsync(clientId);

            }
            return true;
        }
        public async Task<PagingUser> GetUnapprovedUsersAsync(int pageSize, int pageNumber)
        {
            return await _manageUser.GetUnapprovedUsersAsync(pageSize, pageNumber);
        }

        public async Task<bool> ApproveUserAsync(Guid userId)
        {
            return await _manageUser.ApproveUserAsync(userId);
        }
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _manageUser.GetAllRolesAsync();
        }


    }

}
