using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Npgsql;
using TrackIt.Models;
using TrackIt.Repositories;

namespace TrackIt.Data
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<NpgsqlConnection> CreateConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<IEnumerable<Package>> GetAllPackagesByClientAsync(Guid clientId)
        {
            var packages = new List<Package>();
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "SELECT * FROM \"Package\" WHERE \"ClientId\" = @ClientId";
                    command.Parameters.Add(new NpgsqlParameter("@ClientId", clientId));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            packages.Add(new Package
                            {
                                Id = reader.GetGuid(0),
                                ClientId = reader.GetGuid(1),
                                SenderId = reader.GetGuid(2),
                                CourierId = reader.GetGuid(3),
                                Weight = reader.GetFloat(4),
                                Remark = reader.GetString(5),
                                DeliveryAddress = reader.GetString(6),
                                IsActive = reader.GetBoolean(7),
                                CreatedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                UpdatedAt = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                                CreatedBy = reader.IsDBNull(10) ? (Guid?)null : reader.GetGuid(10),
                                UpdatedBy = reader.IsDBNull(11) ? (Guid?)null : reader.GetGuid(11)
                            });
                        }
                    }
                }
            }
            return packages;
        }

        public async Task<Package> GetPackageByIdAsync(Guid packageId)
        {
            Package package = null;
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "SELECT * FROM \"Package\" WHERE \"Id\" = @PackageId";
                    command.Parameters.Add(new NpgsqlParameter("@PackageId", packageId));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            package = new Package
                            {
                                Id = reader.GetGuid(0),
                                ClientId = reader.GetGuid(1),
                                SenderId = reader.GetGuid(2),
                                CourierId = reader.GetGuid(3),
                                Weight = reader.GetFloat(4),
                                Remark = reader.GetString(5),
                                DeliveryAddress = reader.GetString(6),
                                IsActive = reader.GetBoolean(7),
                                CreatedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                UpdatedAt = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                                CreatedBy = reader.IsDBNull(10) ? (Guid?)null : reader.GetGuid(10),
                                UpdatedBy = reader.IsDBNull(11) ? (Guid?)null : reader.GetGuid(11)
                            };
                        }
                    }
                }
            }
            return package;
        }

        public async Task<bool> CancelPackageAsync(Guid packageId)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "UPDATE Package SET IsActive = FALSE WHERE Id = @PackageId";
                    command.Parameters.Add(new NpgsqlParameter("@PackageId", packageId));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> UpdatePackageDeliveryAddressAsync(Guid packageId, string newAddress)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "UPDATE \"Package\" SET \"DeliveryAddress\" = @DeliveryAddress WHERE \"Id\" = @Id";
                    command.Parameters.Add(new NpgsqlParameter("@Id", packageId));
                    command.Parameters.Add(new NpgsqlParameter("@DeliveryAddress", newAddress));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> AddRatingAsync(Guid clientId, int ratingNumber)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var checkCommand = new NpgsqlCommand())
                {
                    checkCommand.Connection = db;
                    checkCommand.CommandText = "SELECT COUNT(1) FROM \"Client\" WHERE \"Id\" = @ClientId";
                    checkCommand.Parameters.Add(new NpgsqlParameter("@ClientId", clientId));

                    var exists = (long)await checkCommand.ExecuteScalarAsync() > 0;
                    if (!exists)
                    {
                        return false;
                    }
                }

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "INSERT INTO \"Rating\" (\"Id\", \"ClientId\", \"RatingNumber\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\",\"CreatedBy\", \"UpdatedBy\") " +
                                          "VALUES (@Id, @ClientId, @RatingNumber, TRUE, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)";

                    var ratingId = Guid.NewGuid();
                    command.Parameters.Add(new NpgsqlParameter("@Id", ratingId));
                    command.Parameters.Add(new NpgsqlParameter("@ClientId", clientId));
                    command.Parameters.Add(new NpgsqlParameter("@RatingNumber", ratingNumber));
                    command.Parameters.Add(new NpgsqlParameter("@CreatedAt", DateTime.UtcNow));
                    command.Parameters.Add(new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow));
                    command.Parameters.Add(new NpgsqlParameter("@CreatedBy", clientId));
                    command.Parameters.Add(new NpgsqlParameter("@UpdatedBy", clientId));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


        public async Task<Rating> AddCommentAsync(Guid ratingId, string comment)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "UPDATE \"Rating\" SET \"Comment\" = @Comment WHERE \"Id\" = @RatingId RETURNING *";
                    command.Parameters.Add(new NpgsqlParameter("@RatingId", ratingId));
                    command.Parameters.Add(new NpgsqlParameter("@Comment", comment));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Rating
                            {
                                Id = reader.GetGuid(0),
                                ClientId = reader.GetGuid(1),
                                PackageId = reader.GetGuid(2),
                                RatingNumber = reader.GetInt32(3),
                                Comment = reader.GetString(4),
                                IsActive = reader.GetBoolean(5),
                                CreatedAt = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                UpdatedAt = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                CreatedBy = reader.IsDBNull(8) ? (Guid?)null : reader.GetGuid(8),
                                UpdatedBy = reader.IsDBNull(9) ? (Guid?)null : reader.GetGuid(9)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<(bool Success, string ChangedSurname, string ChangedAddress)> UpdateClientAsync(Guid clientId, string surname, string address)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = @"
                UPDATE ""Client"" 
                SET 
                    ""Surname"" = COALESCE(@Surname, ""Surname""), 
                    ""Address"" = COALESCE(@Address, ""Address"") 
                WHERE ""Id"" = @Id
                RETURNING ""Surname"", ""Address""";

                    command.Parameters.Add(new NpgsqlParameter("@Id", clientId));
                    command.Parameters.Add(new NpgsqlParameter("@Surname", string.IsNullOrEmpty(surname) ? (object)DBNull.Value : surname));
                    command.Parameters.Add(new NpgsqlParameter("@Address", string.IsNullOrEmpty(address) ? (object)DBNull.Value : address));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var changedSurname = reader["Surname"] as string;
                            var changedAddress = reader["Address"] as string;
                            return (true, changedSurname, changedAddress);
                        }
                        else
                        {
                            return (false, null, null);
                        }
                    }
                }
            }
        }


        public async Task<bool> UpdateUserAsync(Guid userId, string name, string email, string phone, string userName)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "UPDATE \"User\" SET \"Name\" = COALESCE(@Name, \"Name\"), \"Email\" = COALESCE(@Email, \"Email\"), \"Phone\" = COALESCE(@Phone, \"Phone\"), \"UserName\" = COALESCE(@UserName, \"UserName\") WHERE \"Id\" = @Id";
                    command.Parameters.Add(new NpgsqlParameter("@Id", userId));
                    command.Parameters.Add(new NpgsqlParameter("@Name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : name));
                    command.Parameters.Add(new NpgsqlParameter("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email));
                    command.Parameters.Add(new NpgsqlParameter("@Phone", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone));
                    command.Parameters.Add(new NpgsqlParameter("@UserName", string.IsNullOrEmpty(userName) ? (object)DBNull.Value : userName));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task RequestRoleAsync(Guid id, Guid requestedRole)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "UPDATE \"User\" SET \"IsApproved\" = 'false', \"RequestedRole\" = @requestedRole  WHERE \"Id\" = @Id";
                    command.Parameters.Add(new NpgsqlParameter("@Id", id));
                    command.Parameters.Add(new NpgsqlParameter("@requestedRole", requestedRole));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
