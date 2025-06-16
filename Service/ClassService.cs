using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly IRepository<LopHoc, int> _repository;

        public ClassService(IRepository<LopHoc, int> repository)
        {
            _repository = repository;
        }
        // Hiển thị tất cả các lớp học
        public Task<List<LopHoc>> GetAllAsync() => _repository.GetAllAsync();
        // Hiển thị lớp học theo ID
        public Task<LopHoc?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        // Thêm lớp học mới
        public Task<LopHoc?> CreateAsync(LopHoc lopHoc) => _repository.CreateAsync(lopHoc);
        // Cập nhật lớp học theo ID
        public Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc) => _repository.UpdateAsync(id, updatedLopHoc);
        // Xóa lớp học theo ID
        public Task<LopHoc?> DeleteAsync(int id) => _repository.DeleteAsync(id);
        // Phân trang dữ liệu lớp học
        public Task<(List<LopHoc> Items, int TotalCount)> GetPagedAsync(int page, int pageSize) => _repository.GetPagedAsync(page, pageSize);
    }
}