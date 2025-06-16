using StudentAPI.Model;
using Microsoft.Data.SqlClient;

public class UserParameterAdder : IParameterAdder<User>
{
    public void AddParameters(SqlCommand cmd, User user, bool includeId = true)
    {
        if (includeId)
            cmd.Parameters.AddWithValue("@Id", user.Id.ToString());
        cmd.Parameters.AddWithValue("@UserName", user.UserName);
        cmd.Parameters.AddWithValue("@PassWord", user.PassWord);
        cmd.Parameters.AddWithValue("@Role", user.Role);
    }
}