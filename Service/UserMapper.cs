using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

public class UserMapper : IDataMapper<User>
{
    public User Map(SqlDataReader reader)
    {
        return new User
        {
            Id = reader["Id"] is Guid guid ? guid : Guid.Parse(reader["Id"].ToString() ?? Guid.Empty.ToString()),
            UserName = reader["UserName"] as string ?? string.Empty,
            PassWord = reader["PassWord"] as string ?? string.Empty,
            Role = reader["Role"] as string ?? string.Empty
        };
    }
}