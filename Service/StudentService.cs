using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Services
{
    public class StudentService : BaseService, IStudentService
    {
        private static class StoredProcedures
        {
            public const string GetAll = "GETSinhVien";
            public const string GetById = "GETSinhVienById";
            public const string Add = "AddSinhVien";
            public const string Update = "UpdateSinhVien";
            public const string Delete = "DeleteSinhVien";
        }

        private static class Parameters
        {
            public const string StudentId = "@StudentID";
            public const string StudentName = "@StudentName";
            public const string BirthDate = "@BirthDate";
            public const string Gender = "@Gender";
            public const string ClassId = "@ClassID";
        }

        public StudentService(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<List<SinhVien>> GetAllAsync()
        {
            var students = new List<SinhVien>();
            await using var cmd = await CreateCommandAsync(StoredProcedures.GetAll).ConfigureAwait(false);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                students.Add(MapSinhVien(reader));
            }
            return students;
        }

        public async Task<SinhVien?> GetByIdAsync(string studentId)
        {
            await using var cmd = await CreateCommandAsync(StoredProcedures.GetById).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(Parameters.StudentId, studentId);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await reader.ReadAsync().ConfigureAwait(false) ? MapSinhVien(reader) : null;
        }

        public async Task<SinhVien?> CreateAsync(SinhVien sinhVien)
        {
            await using var cmd = await CreateCommandAsync(StoredProcedures.Add).ConfigureAwait(false);
            AddSinhVienParameters(cmd, sinhVien);

            if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
                return await GetByIdAsync(sinhVien.StudentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> UpdateAsync(string studentId, SinhVien updatedSinhVien)
        {
            await using var cmd = await CreateCommandAsync(StoredProcedures.Update).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(Parameters.StudentId, studentId);
            AddSinhVienParameters(cmd, updatedSinhVien, includeId: false);

            if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
                return await GetByIdAsync(studentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> DeleteAsync(string studentId)
        {
            var existing = await GetByIdAsync(studentId).ConfigureAwait(false);
            if (existing == null) return null;

            await using var cmd = await CreateCommandAsync(StoredProcedures.Delete).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(Parameters.StudentId, studentId);

            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0 ? existing : null;
        }

        private static void AddSinhVienParameters(SqlCommand cmd, SinhVien sinhVien, bool includeId = true)
        {
            if (includeId)
                cmd.Parameters.AddWithValue(Parameters.StudentId, sinhVien.StudentId);
            cmd.Parameters.AddWithValue(Parameters.StudentName, sinhVien.StudentName);
            cmd.Parameters.AddWithValue(Parameters.BirthDate, sinhVien.BirthDate);
            cmd.Parameters.AddWithValue(Parameters.Gender, sinhVien.Gender);
            cmd.Parameters.AddWithValue(Parameters.ClassId, sinhVien.ClassId);
        }

        private static SinhVien MapSinhVien(SqlDataReader reader)
        {
            var gender = reader[nameof(SinhVien.Gender)] as string;
            gender = string.IsNullOrWhiteSpace(gender) ? "Other" : gender;

            return new SinhVien
            {
                StudentId = reader[nameof(SinhVien.StudentId)] as string ?? string.Empty,
                StudentName = reader[nameof(SinhVien.StudentName)] as string ?? string.Empty,
                BirthDate = reader[nameof(SinhVien.BirthDate)] is DateTime dt ? dt : Convert.ToDateTime(reader[nameof(SinhVien.BirthDate)]),
                Gender = gender,
                ClassId = reader[nameof(SinhVien.ClassId)] is int id ? id : Convert.ToInt32(reader[nameof(SinhVien.ClassId)])
            };
        }

        public async Task<(List<SinhVien> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var all = await GetAllAsync().ConfigureAwait(false);
            var totalCount = all.Count;
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (items, totalCount);
        }
    }
}