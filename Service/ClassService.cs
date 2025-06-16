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

        public Task<List<LopHoc>> GetAllAsync() => _repository.GetAllAsync();

        public Task<LopHoc?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task<LopHoc?> CreateAsync(LopHoc lopHoc) => _repository.CreateAsync(lopHoc);

        public Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc) => _repository.UpdateAsync(id, updatedLopHoc);

        public Task<LopHoc?> DeleteAsync(int id) => _repository.DeleteAsync(id);

        public Task<(List<LopHoc> Items, int TotalCount)> GetPagedAsync(int page, int pageSize) => _repository.GetPagedAsync(page, pageSize);
    }
}