using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using StudentAPI.Service;

public class GenericRepository<T, TKey> : IRepository<T, TKey>
{
    private readonly BaseService _baseService;
    private readonly string _spGetAll;
    private readonly string _spGetById;
    private readonly string _spAdd;
    private readonly string _spUpdate;
    private readonly string _spDelete;
    private readonly string? _spGetPaged;
    private readonly string _paramId;
    private readonly IDataMapper<T> _mapper;
    private readonly IParameterAdder<T> _parameterAdder;
    private readonly string? _outputIdParamName;

    public GenericRepository(
        BaseService baseService,
        string spGetAll,
        string spGetById,
        string spAdd,
        string spUpdate,
        string spDelete,
        string paramId,
        IDataMapper<T> mapper,
        IParameterAdder<T> parameterAdder,
        string? spGetPaged = null,
        string? outputIdParamName = null)
    {
        _baseService = baseService;
        _spGetAll = spGetAll;
        _spGetById = spGetById;
        _spAdd = spAdd;
        _spUpdate = spUpdate;
        _spDelete = spDelete;
        _paramId = paramId;
        _mapper = mapper;
        _parameterAdder = parameterAdder;
        _spGetPaged = spGetPaged;
        _outputIdParamName = outputIdParamName;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var items = new List<T>();
        await using var cmd = await _baseService.CreateCommandAsync(_spGetAll).ConfigureAwait(false);
        await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            items.Add(_mapper.Map(reader));
        }
        return items;
    }

    public async Task<T?> GetByIdAsync(TKey id)
    {
        await using var cmd = await _baseService.CreateCommandAsync(_spGetById).ConfigureAwait(false);
        cmd.Parameters.AddWithValue(_paramId, id!);
        await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        return await reader.ReadAsync().ConfigureAwait(false) ? _mapper.Map(reader) : default;
    }

    public async Task<T?> CreateAsync(T entity)
    {
        await using var cmd = await _baseService.CreateCommandAsync(_spAdd).ConfigureAwait(false);
        _parameterAdder.AddParameters(cmd, entity, includeId: false);

        // Add output parameter for ID if configured
        if (!string.IsNullOrEmpty(_outputIdParamName))
        {
            var idParam = cmd.Parameters.Add(_outputIdParamName, SqlDbType.VarChar, 50);
            idParam.Direction = ParameterDirection.Output;
        }

        if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
        {
            // If output parameter is set, try to get the new ID and fetch the entity
            if (!string.IsNullOrEmpty(_outputIdParamName))
            {
                var newId = cmd.Parameters[_outputIdParamName].Value;
                if (newId != null && newId != DBNull.Value)
                {
                    return await GetByIdAsync((TKey)newId).ConfigureAwait(false);
                }
            }
        }
        return default;
    }

    public async Task<T?> UpdateAsync(TKey id, T entity)
    {
        await using var cmd = await _baseService.CreateCommandAsync(_spUpdate).ConfigureAwait(false);
        cmd.Parameters.AddWithValue(_paramId, id!);
        _parameterAdder.AddParameters(cmd, entity, includeId: false);
        if (await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0)
            return await GetByIdAsync(id).ConfigureAwait(false);
        return default;
    }

    public async Task<T?> DeleteAsync(TKey id)
    {
        var existing = await GetByIdAsync(id).ConfigureAwait(false);
        if (existing == null) return default;

        await using var cmd = await _baseService.CreateCommandAsync(_spDelete).ConfigureAwait(false);
        cmd.Parameters.AddWithValue(_paramId, id!);
        return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0 ? existing : default;
    }

    public async Task<(List<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        if (!string.IsNullOrEmpty(_spGetPaged))
        {
            var items = new List<T>();
            int totalCount = 0;
            await using var cmd = await _baseService.CreateCommandAsync(_spGetPaged).ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                items.Add(_mapper.Map(reader));
                if (totalCount == 0 && reader["TotalCount"] != DBNull.Value)
                    totalCount = Convert.ToInt32(reader["TotalCount"]);
            }
            return (items, totalCount);
        }
        // Fallback: in-memory paging
        var all = await GetAllAsync().ConfigureAwait(false);
        var total = all.Count;
        var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (paged, total);
    }
    public async Task<T?> GetByUserNameAsync(string userName)
    {
        if (typeof(T).Name == "User")
        {
            await using var cmd = await _baseService.CreateCommandAsync("upLogin").ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@UserName", userName);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await reader.ReadAsync().ConfigureAwait(false) ? _mapper.Map(reader) : default;
        }
        throw new NotImplementedException("GetByUserNameAsync chỉ hỗ trợ cho entity User.");
    }
}