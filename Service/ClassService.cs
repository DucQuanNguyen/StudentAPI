using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;

namespace StudentAPI.Services
{
    public class ClassService
    {
        private readonly string _connectionString;
        private const string SP_GET_ALL = "GETLopHoc";
        private const string SP_GET_BY_ID = "GETLopHocbyId";
        private const string SP_ADD = "AddLopHoc";
        private const string SP_UPDATE = "UpdateLopHoc";
        private const string SP_DELETE = "DeleteLopHoc";
        private const string PARAM_ID = "@Id";
        private const string PARAM_CLASS_NAME = "@ClassName";

        public ClassService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private async Task<SqlCommand> CreateCommandAsync(string storedProcedure)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await connection.OpenAsync().ConfigureAwait(false);
            return command;
        }

        public async Task<List<LopHoc>> GetAllAsync()
        {
            var classes = new List<LopHoc>();
            await using var cmd = await CreateCommandAsync(SP_GET_ALL).ConfigureAwait(false);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                classes.Add(MapLopHoc(reader));
            }
            return classes;
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            await using var cmd = await CreateCommandAsync(SP_GET_BY_ID).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
                return MapLopHoc(reader);
            return null;
        }

        public async Task<LopHoc?> CreateAsync(LopHoc lopHoc)
        {
            await using var cmd = await CreateCommandAsync(SP_ADD).ConfigureAwait(false);
            AddLopHocParameters(cmd, lopHoc, includeId: false);

            var idParam = cmd.Parameters.Add(PARAM_ID, SqlDbType.Int);
            idParam.Direction = ParameterDirection.Output;

            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0 && idParam.Value != DBNull.Value)
            {
                int newId = (int)idParam.Value;
                return await GetByIdAsync(newId).ConfigureAwait(false);
            }
            return null;
        }

        public async Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc)
        {
            await using var cmd = await CreateCommandAsync(SP_UPDATE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            AddLopHocParameters(cmd, updatedLopHoc, includeId: false);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0)
                return await GetByIdAsync(id).ConfigureAwait(false);
            return null;
        }

        public async Task<LopHoc?> DeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id).ConfigureAwait(false);
            if (existing == null) return null;

            await using var cmd = await CreateCommandAsync(SP_DELETE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return i > 0 ? existing : null;
        }

        private void AddLopHocParameters(SqlCommand cmd, LopHoc lopHoc, bool includeId = true)
        {
            if (includeId)
                cmd.Parameters.AddWithValue(PARAM_ID, lopHoc.Id);
            cmd.Parameters.AddWithValue(PARAM_CLASS_NAME, lopHoc.ClassName);
        }

        private LopHoc MapLopHoc(SqlDataReader reader)
        {
            return new LopHoc
            {
                Id = reader["ID"] is int id ? id : Convert.ToInt32(reader["ID"]),
                ClassName = reader["ClassName"] as string ?? string.Empty
            };
        }
    }
}