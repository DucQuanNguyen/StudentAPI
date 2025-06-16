using StudentAPI.Model;
using StudentAPI.Service;

public class StudentService : IStudentService
{
    private readonly IRepository<SinhVien, string> _repository;

    public StudentService(IRepository<SinhVien, string> repository)
    {
        _repository = repository;
    }

    public Task<List<SinhVien>> GetAllAsync() => _repository.GetAllAsync();
    public Task<SinhVien?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
    public Task<SinhVien?> CreateAsync(SinhVien sv) => _repository.CreateAsync(sv);
    public Task<SinhVien?> UpdateAsync(string id, SinhVien sv) => _repository.UpdateAsync(id, sv);
    public Task<SinhVien?> DeleteAsync(string id) => _repository.DeleteAsync(id);
    public Task<(List<SinhVien> Items, int TotalCount)> GetPagedAsync(int page, int pageSize) => _repository.GetPagedAsync(page, pageSize);
}