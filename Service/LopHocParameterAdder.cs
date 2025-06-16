using StudentAPI.Model;
using Microsoft.Data.SqlClient;

public class LopHocParameterAdder : IParameterAdder<LopHoc>
{
    public void AddParameters(SqlCommand cmd, LopHoc lopHoc, bool includeId = true)
    {
        if (includeId)
            cmd.Parameters.AddWithValue("@Id", lopHoc.Id);
        cmd.Parameters.AddWithValue("@ClassName", lopHoc.ClassName);
    }
}