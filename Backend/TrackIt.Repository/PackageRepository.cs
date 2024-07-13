using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using TrackIt.Common;
using System.Threading.Tasks;
using TrackIt.Models;
using TrackIt.Repository.Common;
using System.Text;
using System.Reflection;

namespace TrackIt.Repository
{
    public class PackageRepository : IPackageRepository
    {
        private readonly string _connectionString;

        public PackageRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }


        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT \"Id\", \"Surname\", \"Address\" FROM \"Client\"";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetGuid(0),
                                Surname = reader.GetString(1),
                                Address = reader.GetString(2),
                            });
                        }
                    }
                }
            }

            return clients;
        }


        public async Task<IEnumerable<Courier>> GetAllCouriersAsync() 
        {
            var couriers = new List<Courier>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM \"Courier\"", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        couriers.Add(new Courier
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Surname = reader.GetString(reader.GetOrdinal("Surname"))
                        });
                    }
                }
            }

            return couriers;
        }


        public async Task<Paging> GetAllPackagesBySenderAsync(Guid senderId, Sorting sorting, Paging availablePackages)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var countQuery = @"SELECT COUNT(*) FROM ""Package"" 
               JOIN ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
               JOIN ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
               WHERE ""Package"".""SenderId"" = @SenderId AND ""Package"".""IsActive"" = true ";


                await using (var countCommand = new NpgsqlCommand(countQuery, connection))
                {
                    countCommand.Parameters.AddWithValue("@SenderId", senderId);

                    availablePackages.TotalCount = (int)(long)await countCommand.ExecuteScalarAsync();
                }

                availablePackages.TotalPages = (int)Math.Ceiling((double)availablePackages.TotalCount / availablePackages.PageSize);

                var offset = (availablePackages.PageNumber - 1) * availablePackages.PageSize;


                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    StringBuilder query = new StringBuilder();
                    query.Append(@"
                    SELECT 
                        ""Package"".""Id"", 
                        ""Package"".""ClientId"", 
                        ""Package"".""SenderId"", 
                        ""Package"".""CourierId"", 
                        ""Package"".""Weight"", 
                        ""Package"".""Remark"", 
                        ""Package"".""DeliveryAddress"", 
                        ""Package"".""IsActive"", 
                        ""Package"".""CreatedBy"", 
                        ""Package"".""UpdatedBy"", 
                        ""Package"".""CreatedAt"",
                        ""Package"".""TrackingNumber"",
                        ""DeliveryStatus"".""Status"",
                        ""Rating"".""Id"" AS ""RatingId"",
                        ""Rating"".""ClientId"" AS ""RatingClientId"",
                        ""Rating"".""RatingNumber"", 
                        ""Rating"".""Comment"", 
                        ""Rating"".""IsActive"" AS ""RatingIsActive"", 
                        ""Rating"".""CreatedAt"" AS ""RatingCreatedAt"", 
                        ""Rating"".""UpdatedAt"" AS ""RatingUpdatedAt"", 
                        ""Rating"".""CreatedBy"" AS ""RatingCreatedBy"", 
                        ""Rating"".""UpdatedBy"" AS ""RatingUpdatedBy""
                    FROM 
                        ""Package""
                    JOIN 
                        ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                    JOIN 
                        ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                    LEFT JOIN 
                        ""Rating"" ON ""Package"".""Id"" = ""Rating"".""PackageId""
                    WHERE ""Package"".""SenderId"" = @SenderId AND ""Package"".""IsActive"" = true "
                    );

                    switch (sorting.orderBy.ToLower())
                    {
                        case "createdat":
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                        case "weight":
                            query.Append($" ORDER BY \"Package\".\"Weight\" {sorting.sortOrder}");
                            break;
                        default:
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                    }

                    query.Append(" LIMIT @PageSize OFFSET @Offset");

                    command.Parameters.AddWithValue("@SenderId", senderId);
                    command.CommandText = query.ToString();
                    command.Parameters.AddWithValue("@PageSize", availablePackages.PageSize);
                    command.Parameters.AddWithValue("@Offset", (offset));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Package package = new Package
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ClientId = reader.GetGuid(reader.GetOrdinal("ClientId")),
                                SenderId = reader.GetGuid(reader.GetOrdinal("SenderId")),
                                CourierId = reader.GetGuid(reader.GetOrdinal("CourierId")),
                                Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                                Remark = reader.GetString(reader.GetOrdinal("Remark")),
                                DeliveryAddress = reader.GetString(reader.GetOrdinal("DeliveryAddress")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                DeliveryLog = reader.GetString(reader.GetOrdinal("Status")),
                                TrackingNumber = reader.GetString(reader.GetOrdinal("TrackingNumber"))
                            };


                            availablePackages.Packages.Add(package);

                            if (!reader.IsDBNull(reader.GetOrdinal("RatingId")))
                            {
                                var rating = new Rating
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("RatingId")),
                                    ClientId = reader.GetGuid(reader.GetOrdinal("RatingClientId")),
                                    PackageId = reader.GetGuid(reader.GetOrdinal("Id")),
                                    RatingNumber = reader.GetInt32(reader.GetOrdinal("RatingNumber")),
                                    Comment = reader.GetString(reader.GetOrdinal("Comment")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("RatingIsActive")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("RatingCreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("RatingUpdatedAt")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("RatingCreatedBy")),
                                    UpdatedBy = reader.GetGuid(reader.GetOrdinal("RatingUpdatedBy")),
                                };
                                package.Ratings.Add(rating);
                            }
                        }
                    }
                }
            }

            return availablePackages;
        }


 public async Task<Paging> GetAllPackagesByClientAsync(Guid clientId, Sorting sorting, Paging availablePackages)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var countQuery = @"SELECT COUNT(*) FROM ""Package"" 
               JOIN ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
               JOIN ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
               WHERE ""Package"".""ClientId"" = @ClientId AND ""Package"".""IsActive"" = true ";

                await using (var countCommand = new NpgsqlCommand(countQuery, connection))
                {
                    countCommand.Parameters.AddWithValue("@ClientId", clientId);
                    availablePackages.TotalCount = (int)(long)await countCommand.ExecuteScalarAsync();
                }

                availablePackages.TotalPages = (int)Math.Ceiling((double)availablePackages.TotalCount / availablePackages.PageSize);

                var offset = (availablePackages.PageNumber - 1) * availablePackages.PageSize;


                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    StringBuilder query = new StringBuilder();
                    query.Append(@"
                    SELECT 
                        ""Package"".""Id"", 
                        ""Package"".""ClientId"", 
                        ""Package"".""SenderId"", 
                        ""Package"".""CourierId"", 
                        ""Package"".""Weight"", 
                        ""Package"".""Remark"", 
                        ""Package"".""DeliveryAddress"", 
                        ""Package"".""IsActive"", 
                        ""Package"".""CreatedBy"", 
                        ""Package"".""UpdatedBy"", 
                        ""Package"".""CreatedAt"",
                        ""Package"".""TrackingNumber"",
                        ""DeliveryStatus"".""Status"",
                        ""Rating"".""Id"" AS ""RatingId"",
                        ""Rating"".""ClientId"" AS ""RatingClientId"",
                        ""Rating"".""RatingNumber"", 
                        ""Rating"".""Comment"", 
                        ""Rating"".""IsActive"" AS ""RatingIsActive"", 
                        ""Rating"".""CreatedAt"" AS ""RatingCreatedAt"", 
                        ""Rating"".""UpdatedAt"" AS ""RatingUpdatedAt"", 
                        ""Rating"".""CreatedBy"" AS ""RatingCreatedBy"", 
                        ""Rating"".""UpdatedBy"" AS ""RatingUpdatedBy""
                    FROM 
                        ""Package""
                    JOIN 
                        ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                    JOIN 
                        ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                    LEFT JOIN 
                        ""Rating"" ON ""Package"".""Id"" = ""Rating"".""PackageId""
                    WHERE ""Package"".""ClientId"" = @ClientId AND ""Package"".""IsActive"" = true "

                    );

                    
                    switch (sorting.orderBy.ToLower())
                    {
                        case "createdat":
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                        case "weight":
                            query.Append($" ORDER BY \"Package\".\"Weight\" {sorting.sortOrder}");
                            break;
                        default:
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                    }

                    query.Append(" LIMIT @PageSize OFFSET @Offset");

                    command.CommandText = query.ToString();

                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@PageSize", availablePackages.PageSize);
                    command.Parameters.AddWithValue("@Offset", (offset));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Package package = new Package
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ClientId = reader.GetGuid(reader.GetOrdinal("ClientId")),
                                SenderId = reader.GetGuid(reader.GetOrdinal("SenderId")),
                                CourierId = reader.GetGuid(reader.GetOrdinal("CourierId")),
                                Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                                Remark = reader.GetString(reader.GetOrdinal("Remark")),
                                DeliveryAddress = reader.GetString(reader.GetOrdinal("DeliveryAddress")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                DeliveryLog = reader.GetString(reader.GetOrdinal("Status")),
                                TrackingNumber = reader.GetString(reader.GetOrdinal("TrackingNumber"))
                            };


                            availablePackages.Packages.Add(package);

                            if (!reader.IsDBNull(reader.GetOrdinal("RatingId")))
                            {
                                var rating = new Rating
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("RatingId")),
                                    ClientId = reader.GetGuid(reader.GetOrdinal("RatingClientId")),
                                    PackageId = reader.GetGuid(reader.GetOrdinal("Id")),
                                    RatingNumber = reader.GetInt32(reader.GetOrdinal("RatingNumber")),
                                    Comment = reader.GetString(reader.GetOrdinal("Comment")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("RatingIsActive")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("RatingCreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("RatingUpdatedAt")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("RatingCreatedBy")),
                                    UpdatedBy = reader.GetGuid(reader.GetOrdinal("RatingUpdatedBy")),
                                };
                                package.Ratings.Add(rating);
                            }
                        }
                    }
                }
            }

            return availablePackages;
        }


        public async Task<bool> CreatePackageAsync(Package package)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO ""Package"" (""Id"", ""ClientId"", ""SenderId"", ""CourierId"", ""Weight"", ""Remark"", ""DeliveryAddress"", ""CreatedAt"", ""IsActive"", ""TrackingNumber"", ""CreatedBy"", ""UpdatedBy"")
                VALUES (@Id, @ClientId, @SenderId, @CourierId, @Weight, @Remark, @DeliveryAddress, @CreatedAt, @IsActive, @TrackingNumber, @CreatedBy, @UpdatedBy)";

                    command.Parameters.AddWithValue("@Id", package.Id);
                    command.Parameters.AddWithValue("@ClientId", package.ClientId);
                    command.Parameters.AddWithValue("@SenderId", package.SenderId);
                    command.Parameters.AddWithValue("@CourierId", package.CourierId);
                    command.Parameters.AddWithValue("@Weight", package.Weight);
                    command.Parameters.AddWithValue("@Remark", package.Remark);
                    command.Parameters.AddWithValue("@DeliveryAddress", package.DeliveryAddress);
                    command.Parameters.AddWithValue("@CreatedAt", package.CreatedAt);
                    command.Parameters.AddWithValue("@IsActive", package.IsActive);
                    command.Parameters.AddWithValue("@TrackingNumber", package.TrackingNumber);
                    command.Parameters.AddWithValue("@CreatedBy", package.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedBy", package.UpdatedBy);

                    await command.ExecuteNonQueryAsync();
                }

                // Insert delivery status as Pending for the newly created package
                
                
                var deliveryLogId = Guid.NewGuid();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO ""DeliveryLog"" (""Id"", ""PackageId"", ""DeliveryStatusId"", ""CreatedAt"") 
                VALUES (@Id, @PackageId, @DeliveryStatusId, @CreatedAt)";

                    command.Parameters.AddWithValue("@Id", deliveryLogId);
                    command.Parameters.AddWithValue("@PackageId", package.Id);
                    command.Parameters.AddWithValue("@DeliveryStatusId", Guid.Parse("d50e8400-e29b-41d4-a716-446655440000"));
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
        }


        public async Task<bool> CancelPackageAsync(Guid packageId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {   await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE ""Package"" SET ""IsActive"" = FALSE WHERE ""Id"" = @PackageId";
                    command.Parameters.Add(new NpgsqlParameter("@PackageId", packageId));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> UpdatePackageAsync(Guid packageId, string newAddress, string newRemark)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE \"Package\" SET \"DeliveryAddress\" = @DeliveryAddress, \"Remark\" = @Remark WHERE \"Id\" = @Id";
                    command.Parameters.Add(new NpgsqlParameter("@Id", packageId));
                    command.Parameters.Add(new NpgsqlParameter("@DeliveryAddress", newAddress));
                    command.Parameters.Add(new NpgsqlParameter("@Remark", newRemark));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }



        public async Task<bool> AddRatingAndCommentAsync(Guid clientId, Guid packageId, int ratingNumber, string comment)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var checkCommand = new NpgsqlCommand())
                {
                    checkCommand.Connection = connection;
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
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO \"Rating\" (\"Id\", \"ClientId\",\"PackageId\", \"RatingNumber\",\"Comment\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\",\"CreatedBy\", \"UpdatedBy\") " +
                                          "VALUES (@Id, @ClientId,@PackageId, @RatingNumber,@Comment, TRUE, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)";

                    var ratingId = Guid.NewGuid();
                    command.Parameters.Add(new NpgsqlParameter("@Id", ratingId));
                    command.Parameters.Add(new NpgsqlParameter("PackageId", packageId));
                    command.Parameters.Add(new NpgsqlParameter("@ClientId", clientId));
                    command.Parameters.Add(new NpgsqlParameter("@RatingNumber", ratingNumber));
                    command.Parameters.Add(new NpgsqlParameter("@Comment", comment));
                    command.Parameters.Add(new NpgsqlParameter("@CreatedAt", DateTime.Now));
                    command.Parameters.Add(new NpgsqlParameter("@UpdatedAt", DateTime.Now));
                    command.Parameters.Add(new NpgsqlParameter("@CreatedBy", clientId));
                    command.Parameters.Add(new NpgsqlParameter("@UpdatedBy", clientId));

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


        public async Task<Rating> AddCommentAsync(Guid ratingId, string comment)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
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


        public async Task<Paging> GetAvailablePackagesAsync(Sorting sorting, Paging availablePackages)
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var countQuery = @"SELECT COUNT(*) FROM ""Package"" 
                           JOIN ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                           JOIN ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                           WHERE ""DeliveryStatus"".""Status"" = 'Pending'";

                await using (var countCommand = new NpgsqlCommand(countQuery, connection))
                {
                    availablePackages.TotalCount = (int)(long)await countCommand.ExecuteScalarAsync();
                }

                availablePackages.TotalPages = (int)Math.Ceiling((double)availablePackages.TotalCount / availablePackages.PageSize);

                var offset = (availablePackages.PageNumber - 1) * availablePackages.PageSize;


                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    StringBuilder query = new StringBuilder();
                    query.Append(@"
                    SELECT 
                        ""Package"".""Id"", 
                        ""Package"".""ClientId"", 
                        ""Package"".""SenderId"", 
                        ""Package"".""CourierId"", 
                        ""Package"".""Weight"", 
                        ""Package"".""Remark"", 
                        ""Package"".""DeliveryAddress"", 
                        ""Package"".""IsActive"", 
                        ""Package"".""CreatedBy"", 
                        ""Package"".""UpdatedBy"", 
                        ""Package"".""CreatedAt"",
                        ""Package"".""TrackingNumber"",
                        ""DeliveryStatus"".""Status"",
                        ""Rating"".""Id"" AS ""RatingId"",
                        ""Rating"".""ClientId"" AS ""RatingClientId"",
                        ""Rating"".""RatingNumber"", 
                        ""Rating"".""Comment"", 
                        ""Rating"".""IsActive"" AS ""RatingIsActive"", 
                        ""Rating"".""CreatedAt"" AS ""RatingCreatedAt"", 
                        ""Rating"".""UpdatedAt"" AS ""RatingUpdatedAt"", 
                        ""Rating"".""CreatedBy"" AS ""RatingCreatedBy"", 
                        ""Rating"".""UpdatedBy"" AS ""RatingUpdatedBy""
                    FROM 
                        ""Package""
                    JOIN 
                        ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                    JOIN 
                        ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                    LEFT JOIN 
                        ""Rating"" ON ""Package"".""Id"" = ""Rating"".""PackageId""
                    WHERE 
                        ""DeliveryStatus"".""Status"" = 'Pending'");

                    switch (sorting.orderBy.ToLower())
                    {
                        case "createdat":
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                        case "weight":
                            query.Append($" ORDER BY \"Package\".\"Weight\" {sorting.sortOrder}");
                            break;
                        default:
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                    }

                    query.Append(" LIMIT @PageSize OFFSET @Offset");

                    command.CommandText = query.ToString();
                    command.Parameters.AddWithValue("@PageSize", availablePackages.PageSize);
                    command.Parameters.AddWithValue("@Offset", (offset));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Package package = new Package
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ClientId = reader.GetGuid(reader.GetOrdinal("ClientId")),
                                SenderId = reader.GetGuid(reader.GetOrdinal("SenderId")),
                                CourierId = reader.GetGuid(reader.GetOrdinal("CourierId")),
                                Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                                Remark = reader.GetString(reader.GetOrdinal("Remark")),
                                DeliveryAddress = reader.GetString(reader.GetOrdinal("DeliveryAddress")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                DeliveryLog = reader.GetString(reader.GetOrdinal("Status")),
                                TrackingNumber=reader.GetString(reader.GetOrdinal("TrackingNumber"))
                            };


                            availablePackages.Packages.Add(package);

                            if (!reader.IsDBNull(reader.GetOrdinal("RatingId")))
                            {
                                var rating = new Rating
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("RatingId")),
                                    ClientId = reader.GetGuid(reader.GetOrdinal("RatingClientId")),
                                    PackageId = reader.GetGuid(reader.GetOrdinal("Id")),
                                    RatingNumber = reader.GetInt32(reader.GetOrdinal("RatingNumber")),
                                    Comment = reader.GetString(reader.GetOrdinal("Comment")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("RatingIsActive")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("RatingCreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("RatingUpdatedAt")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("RatingCreatedBy")),
                                    UpdatedBy = reader.GetGuid(reader.GetOrdinal("RatingUpdatedBy")),
                                };
                                package.Ratings.Add(rating);
                            }
                        }
                    }
                }
            }

            return availablePackages;
        }



        public async Task<Package> GetPackageInfoAsync(Guid packageId)
        {

            //nadodati datume i drugo
            using (var connection = new NpgsqlConnection(_connectionString))
            {


                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"
                        SELECT 
                            ""Id"", 
                            ""ClientId"", 
                            ""SenderId"", 
                            ""CourierId"", 
                            ""Weight"", 
                            ""Remark"", 
                            ""DeliveryAddress""
                        FROM 
                            ""Package""
                        WHERE 
                            ""Id"" = @PackageId;";
                    ;

                    command.Parameters.AddWithValue("@PackageId", packageId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Package
                            {
                                Id = reader.GetGuid(0),
                                ClientId = reader.GetGuid(1),
                                SenderId = reader.GetGuid(2),
                                CourierId = reader.GetGuid(3),
                                Weight = reader.GetFloat(4),
                                Remark = reader.GetString(5),
                                DeliveryAddress = reader.GetString(6),

                            };
                        }
                        else
                        {
                            return null;
                        }

                    }

                }
            }
        }






        public async Task<bool> MarkPackageStatusAsync(Guid packageId, string status)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {

                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    if (String.IsNullOrEmpty(status)) { return false; }
                    else
                    {
                        
                        if (status == "Delivered")
                        {
                            
                        command.Connection = connection;
                        command.CommandText = @"
                        UPDATE ""DeliveryLog""
                        SET ""DeliveryStatusId"" = (
                            SELECT ""Id"" 
                            FROM ""DeliveryStatus""
                            WHERE ""Status"" = 'Delivered'
                        )
                        WHERE ""PackageId"" = @PackageId;
                        ";

                        command.Parameters.AddWithValue("@PackageId", packageId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        //await command.ExecuteNonQueryAsync();
                        //return true;
                        return rowsAffected > 0;
                        }

                        if (status == "In Transit") 
                        {
                            {
                                command.Connection = connection;
                                command.CommandText = @"
                        
                        UPDATE ""DeliveryLog""
                        SET ""DeliveryStatusId"" = (
                            SELECT ""Id"" 
                            FROM ""DeliveryStatus""
                            WHERE ""Status"" = 'In Transit'
                        )
                        WHERE ""PackageId"" = @PackageId;";

                        


                                command.Parameters.AddWithValue("@PackageId", packageId);


                                int rowsAffected = await command.ExecuteNonQueryAsync();
                                await command.ExecuteNonQueryAsync();
                                //return true;
                                return rowsAffected > 0;

                            }
                         
                        }
                        else { return false; }
                        //if (status != "Delivered" | status != "In Transit") { return false; }
                    }
                }
            }
        }

        public async Task<bool> MarkAsInTransitAsync(Guid packageId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {


                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        UPDATE ""DeliveryLog""
                        SET ""DeliveryStatusId"" = (
                            SELECT ""Id"" 
                            FROM ""DeliveryStatus""
                            WHERE ""Status"" = 'In Transit'
                        )
                        WHERE ""PackageId"" = @PackageId;";

                    command.Parameters.AddWithValue("@PackageId", packageId);


                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;

                }
            }
        }

        public async Task<Paging> GetInTransitPackagesAsync(Sorting sorting, Paging availablePackages)
        {


            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var countQuery = @"SELECT COUNT(*) FROM ""Package"" 
                           JOIN ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                           JOIN ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                           WHERE ""DeliveryStatus"".""Status"" = 'In Transit'";

                await using (var countCommand = new NpgsqlCommand(countQuery, connection))
                {
                    availablePackages.TotalCount = (int)(long)await countCommand.ExecuteScalarAsync();
                }

                availablePackages.TotalPages = (int)Math.Ceiling((double)availablePackages.TotalCount / availablePackages.PageSize);

                var offset = (availablePackages.PageNumber - 1) * availablePackages.PageSize;


                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    StringBuilder query = new StringBuilder();
                    query.Append(@"
                    SELECT 
                        ""Package"".""Id"", 
                        ""Package"".""ClientId"", 
                        ""Package"".""SenderId"", 
                        ""Package"".""CourierId"", 
                        ""Package"".""Weight"", 
                        ""Package"".""Remark"", 
                        ""Package"".""DeliveryAddress"", 
                        ""Package"".""IsActive"", 
                        ""Package"".""CreatedBy"", 
                        ""Package"".""UpdatedBy"", 
                        ""Package"".""CreatedAt"",
                        ""Package"".""TrackingNumber"",
                        ""DeliveryStatus"".""Status"",
                        ""Rating"".""Id"" AS ""RatingId"",
                        ""Rating"".""ClientId"" AS ""RatingClientId"",
                        ""Rating"".""RatingNumber"", 
                        ""Rating"".""Comment"", 
                        ""Rating"".""IsActive"" AS ""RatingIsActive"", 
                        ""Rating"".""CreatedAt"" AS ""RatingCreatedAt"", 
                        ""Rating"".""UpdatedAt"" AS ""RatingUpdatedAt"", 
                        ""Rating"".""CreatedBy"" AS ""RatingCreatedBy"", 
                        ""Rating"".""UpdatedBy"" AS ""RatingUpdatedBy""
                    FROM 
                        ""Package""
                    JOIN 
                        ""DeliveryLog"" ON ""Package"".""Id"" = ""DeliveryLog"".""PackageId""
                    JOIN 
                        ""DeliveryStatus"" ON ""DeliveryLog"".""DeliveryStatusId"" = ""DeliveryStatus"".""Id""
                    LEFT JOIN 
                        ""Rating"" ON ""Package"".""Id"" = ""Rating"".""PackageId""
                    WHERE 
                        ""DeliveryStatus"".""Status"" = 'In Transit'");

                    switch (sorting.orderBy.ToLower())
                    {
                        case "createdat":
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                        case "weight":
                            query.Append($" ORDER BY \"Package\".\"Weight\" {sorting.sortOrder}");
                            break;
                        default:
                            query.Append($" ORDER BY \"Package\".\"CreatedAt\" {sorting.sortOrder}");
                            break;
                    }

                    query.Append(" LIMIT @PageSize OFFSET @Offset");

                    command.CommandText = query.ToString();
                    command.Parameters.AddWithValue("@PageSize", availablePackages.PageSize);
                    command.Parameters.AddWithValue("@Offset", (offset));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Package package = new Package
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ClientId = reader.GetGuid(reader.GetOrdinal("ClientId")),
                                SenderId = reader.GetGuid(reader.GetOrdinal("SenderId")),
                                CourierId = reader.GetGuid(reader.GetOrdinal("CourierId")),
                                Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                                Remark = reader.GetString(reader.GetOrdinal("Remark")),
                                DeliveryAddress = reader.GetString(reader.GetOrdinal("DeliveryAddress")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetGuid(reader.GetOrdinal("UpdatedBy")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                DeliveryLog = reader.GetString(reader.GetOrdinal("Status")),
                                TrackingNumber = reader.GetString(reader.GetOrdinal("TrackingNumber"))
                            };


                            availablePackages.Packages.Add(package);

                            if (!reader.IsDBNull(reader.GetOrdinal("RatingId")))
                            {
                                var rating = new Rating
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("RatingId")),
                                    ClientId = reader.GetGuid(reader.GetOrdinal("RatingClientId")),
                                    PackageId = reader.GetGuid(reader.GetOrdinal("Id")),
                                    RatingNumber = reader.GetInt32(reader.GetOrdinal("RatingNumber")),
                                    Comment = reader.GetString(reader.GetOrdinal("Comment")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("RatingIsActive")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("RatingCreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("RatingUpdatedAt")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("RatingCreatedBy")),
                                    UpdatedBy = reader.GetGuid(reader.GetOrdinal("RatingUpdatedBy")),
                                };
                                package.Ratings.Add(rating);
                            }
                        }
                    }
                }
            }

            return availablePackages;




        }
    }
}
