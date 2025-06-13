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

        public async Task<List<LopHoc>> GetAllAsync()
        {
            var classes = new List<LopHoc>();
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_GET_ALL, con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                classes.Add(MapLopHoc(reader));
            }
            return classes;
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_GET_BY_ID, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapLopHoc(reader);
            return null;
        }

        public async Task<LopHoc?> CreateAsync(LopHoc lopHoc)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_ADD, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_CLASS_NAME, SqlDbType.NVarChar, 100).Value = lopHoc.ClassName;
            var idParam = cmd.Parameters.Add(PARAM_ID, SqlDbType.Int);
            idParam.Direction = ParameterDirection.Output;

            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            if (i > 0 && idParam.Value != DBNull.Value)
            {
                int newId = (int)idParam.Value;
                return await GetByIdAsync(newId);
            }
            return null;
        }

        public async Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_UPDATE, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
            cmd.Parameters.Add(PARAM_CLASS_NAME, SqlDbType.NVarChar, 100).Value = updatedLopHoc.ClassName;

            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            if (i > 0)
                return await GetByIdAsync(id);
            return null;
        }

        public async Task<LopHoc?> DeleteAsync(int id)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            var existing = await GetByIdAsync(id);
            if (existing == null) return null;

            using SqlCommand cmd = new SqlCommand(SP_DELETE, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            return i > 0 ? existing : null;
        }

        private LopHoc MapLopHoc(SqlDataReader reader)
        {
            return new LopHoc
            {
                Id = Convert.ToInt32(reader["ID"]),
                ClassName = reader["ClassName"].ToString()
            };
        }
    }
}