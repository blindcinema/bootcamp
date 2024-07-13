using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TrackIt.Common;
using TrackIt.Models;

namespace TrackIt.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO \"User\" (\"Id\", \"RoleId\", \"Name\", \"Email\", \"Phone\", \"UserName\", \"Password\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\", \"CreatedBy\", \"UpdatedBy\", \"IsApproved\") VALUES( @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13)", conn)
            {
                Parameters = {
                    new("p1", user.Id),
                    new("p2", Guid.Parse("550e8400-e29b-41d4-a716-446655440000")),
                    new("p3", user.Name),
                    new("p4", user.Email),
                    new("p5",user.Phone),
                    new("p6",user.UserName),
                    new("p7",user.Password),
                    new("p8",user.IsActive),
                    new("p9",user.CreatedAt),
                    new("p10",user.UpdatedAt),
                    new("p11",user.CreatedBy),
                    new("p12",user.UpdatedBy),
                    new("p13", true)

                }
            };
            command.ExecuteNonQuery();
            conn.Close();
            // return should return the created user not the passed
            return user;

        }
        public async Task<User> GetUserLoginAsync(User user)
        {

            var userResult = new User();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT \"User\".\"Id\", \"User\".\"Name\", \"User\".\"UserName\", \"User\".\"RoleId\", \"User\".\"Email\", \"User\".\"Password\", \"Role\".\"Description\", COALESCE(\"Client\".\"Id\", \"Courier\".\"Id\", \"Sender\".\"Id\", \"User\".\"Id\") as \"RoleData\" FROM \"User\" INNER JOIN \"Role\" on \"User\".\"RoleId\" = \"Role\".\"Id\" LEFT JOIN \"Client\" on \"Client\".\"UserId\" = \"User\".\"Id\" LEFT JOIN \"Sender\" on \"Sender\".\"UserId\" = \"User\".\"Id\" LEFT join \"Courier\" on \"Courier\".\"UserId\" = \"User\".\"Id\" WHERE \"User\".\"UserName\" = @p1;", conn)
            {
                Parameters =
                {
                    new("p1", user.UserName)
                }
            };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                userResult = new User { Id = reader.GetGuid(0), Name = reader.GetString(1), UserName = reader.GetString(2), RoleId = reader.GetGuid(3), Email = reader.GetString(4), Password = reader.GetString(5), Role = reader.GetString(6), RoleData = reader.GetGuid(7)  };
                //Console.WriteLine(JsonSerializer.Serialize(userResult));
            }
            //Console.Write("fired");
            conn.Close();
            return userResult;
        }

        public async Task<User> EditUserAsync(User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE \"User\" SET \"RoleId\" = @p2, \"Name\" = @p3, \"Email\" = @p4, \"Phone\" = @p5, \"UserName\" = @p6, \"Password\" = @p7, \"IsActive\" = @p8, \"CreatedAt\" = @p9, \"UpdatedAt\" = @p10, \"CreatedBy\" = @p11, \"UpdatedBy\" = @p12, \"IsApproved\" = @p13  WHERE \"Id\" = @p1", conn)
            {
                Parameters = {
                    new("p1", user.Id),
                    new("p2",user.RoleId),
                    new("p3", user.Name),
                    new("p4", user.Email),
                    new("p5",user.Phone),
                    new("p6",user.UserName),
                    new("p7",user.Password),
                    new("p8",user.IsActive),
                    new("p9",user.CreatedAt),
                    new("p10",user.UpdatedAt),
                    new("p11",user.CreatedBy),
                    new("p12",user.UpdatedBy),
                    new("p13", user.IsApproved)
                }
            };
            command.ExecuteNonQuery();
            conn.Close();
            return user;
        }

        public async Task UpdatePasswordAsync(Guid id, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE \"User\" SET \"Password\" = @password WHERE \"Id\" = @p1", conn)
            {
                Parameters =
                {
                    new("password", password),
                    new("p1", id)
                }
            };



            command.ExecuteNonQuery();
            conn.Close();


        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {

            var userResult = new User();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM  \"User\" INNER JOIN \"Role\" ON \"RoleId\" = \"Role\".\"Id\" WHERE \"User\".\"Id\" = @id", conn)
            {
                Parameters =
                {
                    new("id", id)
                }
            };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                userResult = new User { Id = reader.GetGuid(0), RoleId = reader.GetGuid(1), Name = reader.GetString(2), Email = reader.GetString(3), Phone = reader.GetString(4), UserName = reader.GetString(5), Password = reader.GetString(6), Role = reader.GetString(14) };
                //Console.WriteLine(JsonSerializer.Serialize(userResult));
            }
            //Console.Write("fired");
            conn.Close();
            return userResult;
        }



        public async Task<bool> UpdateUserAsync(User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE \"User\" SET \"RoleId\" = @roleId, \"IsApproved\" = @isApproved, \"UpdatedAt\" = @updatedAt, \"UpdatedBy\" = @updatedBy WHERE \"Id\" = @userId", connection))
                {
                    command.Parameters.AddWithValue("roleId", user.RoleId);
                    command.Parameters.AddWithValue("isApproved", user.IsApproved);
                    command.Parameters.AddWithValue("updatedAt", user.UpdatedAt);
                    command.Parameters.AddWithValue("updatedBy", user.UpdatedBy);
                    command.Parameters.AddWithValue("userId", user.Id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {

            List<User> userList = new List<User>();
            //var userResult = new User();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM  \"User\" INNER JOIN \"Role\" ON \"RoleId\" = \"Role\".\"Id\"", conn);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                var userResult = new User { Id = reader.GetGuid(reader.GetOrdinal("Id")), 
                    RoleId = reader.GetGuid(reader.GetOrdinal("RoleId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    Role = reader.GetString(reader.GetOrdinal("Description")) };
                userList.Add(userResult);
                //Console.WriteLine(JsonSerializer.Serialize(userResult));
            }
            //Console.Write("fired");
            conn.Close();
            return userList;
        }

        public async Task<List<User>> HardDeleteUserAsync(Guid id)
        { // TRENUTNO SAMO VRATI LISTU NAZAD

            List<User> userList = new List<User>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("DELETE FROM  \"User\" WHERE \"Id\" = @p1", conn) { Parameters = { new("p1", id) } };

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                var userResult = new User { Id = reader.GetGuid(0), RoleId = reader.GetGuid(1), Name = reader.GetString(2), Email = reader.GetString(3), Phone = reader.GetString(4), UserName = reader.GetString(5), Password = reader.GetString(6), Role = reader.GetString(14) };
                userList.Add(userResult);
                //Console.WriteLine(JsonSerializer.Serialize(userResult));
            }
            //Console.Write("fired");
            conn.Close();
            return userList;
        }


        public async Task<List<User>> SoftDeleteUserAsync(Guid id)
        { // TRENUTNO SAMO VRATI LISTU NAZAD

            List<User> userList = new List<User>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE \"User\" SET \"IsActive\" = false WHERE \"Id\" = @id;", conn){ Parameters = { new("id",id)} };

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                var userResult = new User { Id = reader.GetGuid(0), RoleId = reader.GetGuid(1), Name = reader.GetString(2), Email = reader.GetString(3), Phone = reader.GetString(4), UserName = reader.GetString(5), Password = reader.GetString(6), Role = reader.GetString(14) };
                userList.Add(userResult);
                //Console.WriteLine(JsonSerializer.Serialize(userResult));
            }
            //Console.Write("fired");
            conn.Close();
            return userList;
        }

        public async Task<Role> GetRoleByDescriptionAsync(string description)
        {
            Role role = null;
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM \"Role\" WHERE \"Description\" = @description", connection))
                {
                    command.Parameters.AddWithValue("description", description);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            role = new Role
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy"))
                            };
                        }
                    }
                }
                connection.Close();
            }

            return role;
        }

        public async Task<PagingUser> GetUnapprovedUsersAsync(int pageSize, int pageNumber)
        {
            var pagedUsers = new PagingUser
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the total count of unapproved users
                var countQuery = @"SELECT COUNT(*) FROM ""User"" WHERE ""IsApproved"" = false";
                using (var countCommand = new NpgsqlCommand(countQuery, connection))
                {
                    pagedUsers.TotalCount = (int)(long)await countCommand.ExecuteScalarAsync();
                }

                // Calculate the offset
                var offset = (pageNumber - 1) * pageSize;

                // Get the paged unapproved users
                var query = @"
            SELECT u.""Id"" as ""UserId"", u.""Name"", u.""Email"", u.""Phone"", u.""UserName"", u.""IsApproved"", u.""IsActive"",
                   r.""Id"" as ""RoleId"", r.""Description""
            FROM ""User"" u
            JOIN ""Role"" r ON u.""RoleId"" = r.""Id""
            WHERE u.""IsApproved"" = false
            ORDER BY u.""Name""
            LIMIT @pageSize OFFSET @offset";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("pageSize", pageSize);
                    command.Parameters.AddWithValue("offset", offset);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("UserId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                                Role = reader.GetString(reader.GetOrdinal("Description"))
                            };

                            pagedUsers.Users.Add(user);
                        }
                    }
                }
                connection.Close();
            }

            return pagedUsers;
        }




        public async Task<bool> UpdateUserRoleAsync(User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    UPDATE ""User""
                    SET ""RoleId"" = @newRoleId, ""IsApproved"" = false, ""UpdatedAt"" = @updatedAt
                    WHERE ""Id"" = @userId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("newRoleId", user.RoleId);
                    command.Parameters.AddWithValue("updatedAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("userId", user.Id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteClientAsync(Guid clientId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("DELETE FROM \"Client\" WHERE \"Id\" = @ClientId;", connection))
                {
                    command.Parameters.AddWithValue("clientId", clientId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> AddSenderAsync(Sender sender)
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO \"Sender\" (\"Id\", \"Address\", \"UserId\") VALUES (@id, @address, @userId)", connection))
                {
                    command.Parameters.AddWithValue("id", sender.Id);
                    command.Parameters.AddWithValue("address", sender.Address);
                    command.Parameters.AddWithValue("userId", sender.UserId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<Client> GetClientByIdAsync(Guid clientId)
        {
            Client client = null;
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM \"Client\" WHERE \"Id\" = @clientId", connection))
                {
                    command.Parameters.AddWithValue("clientId", clientId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            client = new Client
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy"))
                            };
                        }
                    }
                }
                connection.Close();
            }
            return client;
        }
        public async Task<bool> AddCourierAsync(Courier courier)
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO \"Courier\" (\"Id\", \"Surname\", \"UserId\") VALUES (@id, @surname, @userId)", connection))
                {
                    command.Parameters.AddWithValue("id", courier.Id);
                    command.Parameters.AddWithValue("surname", courier.Surname);
                    command.Parameters.AddWithValue("userId", courier.UserId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> ApproveUserAsync(Guid userId)
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("Update \"User\" SET \"IsApproved\"= True, \"RoleId\" = \"User\".\"RequestedRole\", \"RequestedRole\" = NULL WHERE \"Id\" = @Id", connection))
                {
                    command.Parameters.AddWithValue("id", userId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    return rowsAffected > 0;
                }
            }
        }
        public async Task<List<Role>> GetAllRolesAsync()
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT \"Id\", \"Description\" FROM \"Role\"", connection))
                {
                    var roles = new List<Role>();
                    using (var reader = await command.ExecuteReaderAsync()) {

                        while (await reader.ReadAsync()) {
                            var role = new Role
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            };
                            roles.Add(role);

                            
                        }
                    }
                        connection.Close();
                        return roles;
                    }
                    

                }
            }
        }
    }

