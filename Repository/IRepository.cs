using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepository<T, TKey>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(TKey id);
    Task<T?> CreateAsync(T entity);
    Task<T?> UpdateAsync(TKey id, T entity);
    Task<T?> DeleteAsync(TKey id);
    Task<(List<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<T?> GetByUserNameAsync(string userName);
}