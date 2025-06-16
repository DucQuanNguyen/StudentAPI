using StudentAPI.Model;
using StudentAPI.Service;

public class StudentService : IStudentService
{
    private readonly IRepository<SinhVien, string> _repository;

    public StudentService(IRepository<SinhVien, string> repository)
    {
        _repository = repository;
    }
    // Hiển thị tất cả sinh viên
    public Task<List<SinhVien>> GetAllAsync() => _repository.GetAllAsync();
    // Hiển thị sinh viên theo ID
    public Task<SinhVien?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
    // Thêm sinh viên mới
    public Task<SinhVien?> CreateAsync(SinhVien sv) => _repository.CreateAsync(sv);
    //Cập nhật sinh viên theo ID
    public Task<SinhVien?> UpdateAsync(string id, SinhVien sv) => _repository.UpdateAsync(id, sv);
    // Xóa sinh viên theo ID
    public Task<SinhVien?> DeleteAsync(string id) => _repository.DeleteAsync(id);
    // Phân trang dữ liệu sinh viên
    public Task<(List<SinhVien> Items, int TotalCount)> GetPagedAsync(int page, int pageSize) => _repository.GetPagedAsync(page, pageSize);
}