using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Services
{
    public class ClassService : BaseService, IClassService
    {
        private const string SP_GET_ALL = "GETLopHoc";
        private const string SP_GET_BY_ID = "GETLopHocbyId";
        private const string SP_ADD = "AddLopHoc";
        private const string SP_UPDATE = "UpdateLopHoc";
        private const string SP_DELETE = "DeleteLopHoc";
        private const string PARAM_ID = "@Id";
        private const string PARAM_CLASS_NAME = "@ClassName";

        private readonly IDataMapper<LopHoc> _mapper;
        private readonly IParameterAdder<LopHoc> _parameterAdder;

        public ClassService(IConfiguration configuration)
            : base(configuration)
        {
            _mapper = new LopHocMapper();
            _parameterAdder = new LopHocParameterAdder();
        }

        public async Task<List<LopHoc>> GetAllAsync()
        {
            var classes = new List<LopHoc>();
            await using var cmd = await CreateCommandAsync(SP_GET_ALL).ConfigureAwait(false);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                classes.Add(_mapper.Map(reader));
            }
            return classes;
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            await using var cmd = await CreateCommandAsync(SP_GET_BY_ID).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
                return _mapper.Map(reader);
            return null;
        }

        public async Task<LopHoc?> CreateAsync(LopHoc lopHoc)
        {
            await using var cmd = await CreateCommandAsync(SP_ADD).ConfigureAwait(false);
            _parameterAdder.AddParameters(cmd, lopHoc, includeId: false);

            var idParam = cmd.Parameters.Add(PARAM_ID, SqlDbType.Int);
            idParam.Direction = ParameterDirection.Output;

            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0 && idParam.Value != DBNull.Value)
            {
                int newId = (int)idParam.Value;
                return await GetByIdAsync(newId).ConfigureAwait(false);
            }
            return null;
        }

        public async Task<LopHoc?> UpdateAsync(int id, LopHoc updatedLopHoc)
        {
            await using var cmd = await CreateCommandAsync(SP_UPDATE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            _parameterAdder.AddParameters(cmd, updatedLopHoc, includeId: false);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (i > 0)
                return await GetByIdAsync(id).ConfigureAwait(false);
            return null;
        }

        public async Task<LopHoc?> DeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id).ConfigureAwait(false);
            if (existing == null) return null;

            await using var cmd = await CreateCommandAsync(SP_DELETE).ConfigureAwait(false);
            cmd.Parameters.AddWithValue(PARAM_ID, id);
            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return i > 0 ? existing : null;
        }

        public async Task<(List<LopHoc> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var all = await GetAllAsync();
            var totalCount = all.Count;
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (items, totalCount);
        }
    }
}