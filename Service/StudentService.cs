using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Services
{
    public class StudentService : BaseService, IStudentService
    {
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
            : base(configuration)
        {
        }

        public async Task<List<SinhVien>> GetAllAsync()
        {
            var students = new List<SinhVien>();
            await using var cmd = await CreateCommandAsync(SP_GET_ALL).ConfigureAwait(false);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                students.Add(MapSinhVien(reader));
            }
            return students;
        }

        public async Task<SinhVien?> GetByIdAsync(string studentId)
        {
            await using var cmd = await CreateCommandAsync(SP_GET_BY_ID).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
                return MapSinhVien(reader);
            return null;
        }

        public async Task<SinhVien?> CreateAsync(SinhVien sinhVien)
        {
            await using var cmd = await CreateCommandAsync(SP_ADD).ConfigureAwait(false);
            AddSinhVienParameters(cmd, sinhVien);

            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0)
                return await GetByIdAsync(sinhVien.StudentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> UpdateAsync(string studentId, SinhVien updatedSinhVien)
        {
            await using var cmd = await CreateCommandAsync(SP_UPDATE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
            AddSinhVienParameters(cmd, updatedSinhVien, includeId: false);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0)
                return await GetByIdAsync(studentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> DeleteAsync(string studentId)
        {
            var existing = await GetByIdAsync(studentId).ConfigureAwait(false);
            if (existing == null) return null;

            await using var cmd = await CreateCommandAsync(SP_DELETE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return i > 0 ? existing : null;
        }

        private void AddSinhVienParameters(SqlCommand cmd, SinhVien sinhVien, bool includeId = true)
        {
            if (includeId)
                cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, sinhVien.StudentId);
            cmd.Parameters.AddWithValue(PARAM_STUDENT_NAME, sinhVien.StudentName);
            cmd.Parameters.AddWithValue(PARAM_BIRTH_DATE, sinhVien.BirthDate);
            cmd.Parameters.AddWithValue(PARAM_GENDER, sinhVien.Gender);
            cmd.Parameters.AddWithValue(PARAM_CLASS_ID, sinhVien.ClassId);
        }

        private SinhVien MapSinhVien(SqlDataReader reader)
        {
            return new SinhVien
            {
                StudentId = reader["StudentId"] as string ?? string.Empty,
                StudentName = reader["StudentName"] as string ?? string.Empty,
                BirthDate = reader["BirthDate"] is DateTime dt ? dt : Convert.ToDateTime(reader["BirthDate"]),
                Gender = reader["Gender"] as string ?? string.Empty,
                ClassId = reader["ClassId"] is int id ? id : Convert.ToInt32(reader["ClassId"])
            };
        }
    }
}