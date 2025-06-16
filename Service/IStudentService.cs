using StudentAPI.Model;

namespace StudentAPI.Service
{
    public interface IStudentService
    {
        Task<List<SinhVien>> GetAllAsync();
        Task<SinhVien?> GetByIdAsync(string studentId);
        Task<SinhVien?> CreateAsync(SinhVien sinhVien);
        Task<SinhVien?> UpdateAsync(string studentId, SinhVien updatedSinhVien);
        Task<SinhVien?> DeleteAsync(string studentId);
        Task<(List<SinhVien> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}
