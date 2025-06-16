using Microsoft.Data.SqlClient;

namespace StudentAPI.Service
{
    public interface IDataMapper<T>
    {
        T Map(SqlDataReader reader);
    }
}
