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

namespace TrackIt.Repository
{
    public class CourierRepository : ICourierRepository
    {
        private readonly string _connectionString;

        public CourierRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }
        private async Task<NpgsqlConnection> CreateConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        public async Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync()
        {
            var paymentMethods = new List<PaymentMethod>();
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = "SELECT * FROM \"PaymentMethod\" WHERE \"IsActive\" = TRUE";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            paymentMethods.Add(new PaymentMethod
                            {
                                Id = reader.GetGuid(0),
                                PaymentId = reader.GetGuid(1),
                                Type = reader.GetString(2),
                                IsActive = reader.GetBoolean(3),
                                CreatedAt = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                UpdatedAt = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                CreatedBy = reader.IsDBNull(6) ? (Guid?)null : reader.GetGuid(6),
                                UpdatedBy = reader.IsDBNull(7) ? (Guid?)null : reader.GetGuid(7)
                            });
                        }
                    }
                }
            }
            return paymentMethods;
        }

        public async Task<bool> MarkPaymentAsPaidAsync(Guid paymentId)
        {
            using (var db = await CreateConnectionAsync())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = db;
                    command.CommandText = @"UPDATE ""PaymentMethod""
                       SET ""IsActive"" = FALSE, ""UpdatedAt"" = @UpdatedAt
                       WHERE ""PaymentId"" = @PaymentId";

                    
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@PaymentId", paymentId);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
      
    }
}
