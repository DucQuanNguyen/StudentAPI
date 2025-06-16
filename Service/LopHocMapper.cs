using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

public class LopHocMapper : IDataMapper<LopHoc>
{
    public LopHoc Map(SqlDataReader reader)
    {
        return new LopHoc
        {
            Id = reader["ID"] is int id ? id : Convert.ToInt32(reader["ID"]),
            ClassName = reader["ClassName"] as string ?? string.Empty
        };
    }
}