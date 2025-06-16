using StudentAPI.Model;

namespace StudentAPI.Service
{
    public interface IClassService
    {
        Task<List<LopHoc>> GetAllAsync();
        Task<LopHoc?> GetByIdAsync(int id);
        Task<LopHoc?> CreateAsync(LopHoc lopHoc);
        Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc);
        Task<LopHoc?> DeleteAsync(int id);
        Task<(List<LopHoc> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}
