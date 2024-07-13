using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackIt.Common;
using TrackIt.Models;

namespace TrackIt.Service
{
    public interface IUserService
    {
        public Task<User> CreateUserAsync(UserDto request);
        public Task<User> GetUserLoginAsync(UserDto request);
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(Guid id);
        public Task<List<User>> SoftDeleteUserAsync(Guid id);
        public Task<List<User>> HardDeleteUserAsync(Guid id);
        public Task<User> EditUserAsync(UserDto request);
        public Task UpdatePasswordAsync(Guid id, string password);
        Task<bool> ChangeClientRoleAsync(Guid clientId, string newRole);
        Task<PagingUser> GetUnapprovedUsersAsync(int pageSize, int pageNumber);
        Task<bool> ApproveUserAsync(Guid userId);
        Task<List<Role>> GetAllRolesAsync();
    }
}
