using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentAPI.Service
{
    public abstract class BaseService
    {
        private readonly string _connectionString;

        protected BaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected async Task<SqlCommand> CreateCommandAsync(string storedProcedure)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await connection.OpenAsync().ConfigureAwait(false);
            return command;
        }
    }
}