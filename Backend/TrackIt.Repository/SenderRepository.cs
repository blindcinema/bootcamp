using Npgsql;
using System.Threading.Tasks;
using System;
using TrackIt.Models;

public class SenderRepository : ISenderRepository
{
   private readonly string _connectionString;

    public SenderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<bool> CreatePackageAsync(Package package)
    {
        using (var db = await CreateConnectionAsync())
        {
            using (var command = new NpgsqlCommand())
            {
                command.Connection = db;
                command.CommandText = @"
                INSERT INTO ""Package"" (""Id"", ""SenderId"", ""Weight"", ""Remark"", ""DeliveryAddress"", ""CreatedAt"", ""IsActive"") 
                VALUES (@Id, @SenderId, @Weight, @Remark, @DeliveryAddress, @CreatedAt, @IsActive)";

                command.Parameters.AddWithValue("@Id", package.Id);
                command.Parameters.AddWithValue("@SenderId", package.SenderId);
                command.Parameters.AddWithValue("@Weight", package.Weight);
                command.Parameters.AddWithValue("@Remark", package.Remark);
                command.Parameters.AddWithValue("@DeliveryAddress", package.DeliveryAddress);
                command.Parameters.AddWithValue("@CreatedAt", package.CreatedAt);
                command.Parameters.AddWithValue("@IsActive", package.IsActive);

                await command.ExecuteNonQueryAsync();
            }

            // Insert delivery status as Pending for the newly created package
            var deliveryStatusId = Guid.NewGuid();
            using (var command = new NpgsqlCommand())
            {
                command.Connection = db;
                command.CommandText = @"
                INSERT INTO ""DeliveryStatus"" (""Id"", ""Status"", ""Date"", ""IsActive"") 
                VALUES (@Id, @Status, @Date, @IsActive)";

                command.Parameters.AddWithValue("@Id", deliveryStatusId);
                command.Parameters.AddWithValue("@Status", "Pending");
                command.Parameters.AddWithValue("@Date", DateTime.UtcNow);
                command.Parameters.AddWithValue("@IsActive", true);

                await command.ExecuteNonQueryAsync();
            }

            // Insert delivery log with initial status as Pending
            var deliveryLogId = Guid.NewGuid();
            using (var command = new NpgsqlCommand())
            {
                command.Connection = db;
                command.CommandText = @"
                INSERT INTO ""DeliveryLog"" (""Id"", ""PackageId"", ""DeliveryStatusId"", ""CreatedAt"") 
                VALUES (@Id, @PackageId, @DeliveryStatusId, @CreatedAt)";

                command.Parameters.AddWithValue("@Id", deliveryLogId);
                command.Parameters.AddWithValue("@PackageId", package.Id);
                command.Parameters.AddWithValue("@DeliveryStatusId", deliveryStatusId);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }

            return true;
        }
    }



    public async Task<string> GetPackageStatusAsync(Guid packageId)
    {
        using (var db = await CreateConnectionAsync())
        {
            using (var command = new NpgsqlCommand())
            {
                command.Connection = db;
                command.CommandText = @"SELECT ds.""Status"" FROM ""DeliveryLog"" dl 
                                        JOIN ""DeliveryStatus"" ds ON dl.""DeliveryStatusId"" = ds.""Id"" 
                                        WHERE dl.""PackageId"" = @PackageId 
                                        ORDER BY dl.""CreatedAt"" DESC LIMIT 1";
                command.Parameters.AddWithValue("@PackageId", packageId);

                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            }
        }
    }
}
