using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;

namespace StudentAPI.Services
{
    public class StudentService
    {
        private readonly string _connectionString;
        private const string SP_GET_ALL = "GETSinhVien";
        private const string SP_GET_BY_ID = "GETSinhVienById";
        private const string SP_ADD = "AddSinhVien";
        private const string SP_UPDATE = "UpdateSinhVien";
        private const string SP_DELETE = "DeleteSinhVien";

        private const string PARAM_STUDENT_ID = "@StudentID";
        private const string PARAM_STUDENT_NAME = "@StudentName";
        private const string PARAM_BIRTH_DATE = "@BirthDate";
        private const string PARAM_GENDER = "@Gender";
        private const string PARAM_CLASS_ID = "@ClassID";

        public StudentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<SinhVien>> GetAllAsync()
        {
            var students = new List<SinhVien>();
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_GET_ALL, con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                students.Add(MapSinhVien(reader));
            }
            return students;
        }

        public async Task<SinhVien?> GetByIdAsync(string studentId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_GET_BY_ID, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_STUDENT_ID, SqlDbType.NVarChar, 50).Value = studentId;
            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapSinhVien(reader);
            return null;
        }

        public async Task<SinhVien?> CreateAsync(SinhVien sinhVien)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_ADD, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_STUDENT_ID, SqlDbType.NVarChar, 50).Value = sinhVien.StudentId;
            cmd.Parameters.Add(PARAM_STUDENT_NAME, SqlDbType.NVarChar, 100).Value = sinhVien.StudentName;
            cmd.Parameters.Add(PARAM_BIRTH_DATE, SqlDbType.Date).Value = sinhVien.BirthDate;
            cmd.Parameters.Add(PARAM_GENDER, SqlDbType.NVarChar, 10).Value = sinhVien.Gender;
            cmd.Parameters.Add(PARAM_CLASS_ID, SqlDbType.Int).Value = sinhVien.ClassId;

            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            if (i > 0)
                return await GetByIdAsync(sinhVien.StudentId);
            return null;
        }

        public async Task<SinhVien?> UpdateAsync(string studentId, SinhVien updatedSinhVien)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(SP_UPDATE, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_STUDENT_ID, SqlDbType.NVarChar, 50).Value = studentId;
            cmd.Parameters.Add(PARAM_STUDENT_NAME, SqlDbType.NVarChar, 100).Value = updatedSinhVien.StudentName;
            cmd.Parameters.Add(PARAM_BIRTH_DATE, SqlDbType.Date).Value = updatedSinhVien.BirthDate;
            cmd.Parameters.Add(PARAM_GENDER, SqlDbType.NVarChar, 10).Value = updatedSinhVien.Gender;
            cmd.Parameters.Add(PARAM_CLASS_ID, SqlDbType.Int).Value = updatedSinhVien.ClassId;

            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            if (i > 0)
                return await GetByIdAsync(studentId);
            return null;
        }

        public async Task<SinhVien?> DeleteAsync(string studentId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            // Get before delete
            var existing = await GetByIdAsync(studentId);
            if (existing == null) return null;

            using SqlCommand cmd = new SqlCommand(SP_DELETE, con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(PARAM_STUDENT_ID, SqlDbType.NVarChar, 50).Value = studentId;
            await con.OpenAsync();
            int i = await cmd.ExecuteNonQueryAsync();
            return i > 0 ? existing : null;
        }

        private SinhVien MapSinhVien(SqlDataReader reader)
        {
            return new SinhVien
            {
                StudentId = reader["StudentId"].ToString(),
                StudentName = reader["StudentName"].ToString(),
                BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                Gender = reader["Gender"].ToString(),
                ClassId = Convert.ToInt32(reader["ClassId"])
            };
        }
    }
}