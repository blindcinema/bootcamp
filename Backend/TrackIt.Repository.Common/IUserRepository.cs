using TrackIt.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using TrackIt.Common;


public interface IUserRepository
    {
        public Task<User> CreateUserAsync(User user);
        public Task<User> GetUserLoginAsync(User user);
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(Guid id);
        Task<bool> UpdateUserAsync(User user);
    public Task<List<User>> SoftDeleteUserAsync(Guid id);
        public Task<List<User>> HardDeleteUserAsync(Guid id);
        public Task<User> EditUserAsync(User user);
        public Task UpdatePasswordAsync(Guid id, string password);
        Task<Role> GetRoleByDescriptionAsync(string description);
        Task<PagingUser> GetUnapprovedUsersAsync(int pageSize, int pageNumber);
        Task<bool> UpdateUserRoleAsync(User user);
        Task<bool> DeleteClientAsync(Guid clientId);
        Task<bool> AddSenderAsync(Sender sender);
        Task<Client> GetClientByIdAsync(Guid clientId);
        Task<bool> AddCourierAsync(Courier courier);
        Task<bool> ApproveUserAsync(Guid userId);
        Task<List<Role>> GetAllRolesAsync();

}

