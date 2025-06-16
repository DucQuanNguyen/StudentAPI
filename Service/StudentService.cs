using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Services
{
    public class StudentService : BaseService, IStudentService
    {
        private readonly IDataMapper<SinhVien> _mapper;
        private readonly IParameterAdder<SinhVien> _parameterAdder;

        public StudentService(IConfiguration configuration)
            : base(configuration)
        {
            _mapper = new SinhVienMapper();
            _parameterAdder = new SinhVienParameterAdder();
        }

        public async Task<List<SinhVien>> GetAllAsync()
        {
            var students = new List<SinhVien>();
            await using var cmd = await CreateCommandAsync("GETSinhVien").ConfigureAwait(false);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                students.Add(_mapper.Map(reader));
            }
            return students;
        }

        public async Task<SinhVien?> GetByIdAsync(string studentId)
        {
            await using var cmd = await CreateCommandAsync("GETSinhVienById").ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@StudentID", studentId);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await reader.ReadAsync().ConfigureAwait(false) ? _mapper.Map(reader) : null;
        }

        public async Task<SinhVien?> CreateAsync(SinhVien sinhVien)
        {
            await using var cmd = await CreateCommandAsync("AddSinhVien").ConfigureAwait(false);
            _parameterAdder.AddParameters(cmd, sinhVien);

            if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
                return await GetByIdAsync(sinhVien.StudentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> UpdateAsync(string studentId, SinhVien updatedSinhVien)
        {
            await using var cmd = await CreateCommandAsync("UpdateSinhVien").ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@StudentID", studentId);
            _parameterAdder.AddParameters(cmd, updatedSinhVien, includeId: false);

            if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
                return await GetByIdAsync(studentId).ConfigureAwait(false);
            return null;
        }

        public async Task<SinhVien?> DeleteAsync(string studentId)
        {
            var existing = await GetByIdAsync(studentId).ConfigureAwait(false);
            if (existing == null) return null;

            await using var cmd = await CreateCommandAsync("DeleteSinhVien").ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@StudentID", studentId);

            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0 ? existing : null;
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