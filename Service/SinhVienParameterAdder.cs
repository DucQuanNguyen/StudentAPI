using StudentAPI.Model;
using Microsoft.Data.SqlClient;

public class SinhVienParameterAdder : IParameterAdder<SinhVien>
{
    public void AddParameters(SqlCommand cmd, SinhVien sinhVien, bool includeId = true)
    {
        if (includeId)
            cmd.Parameters.AddWithValue("@StudentID", sinhVien.StudentId);
        cmd.Parameters.AddWithValue("@StudentName", sinhVien.StudentName);
        cmd.Parameters.AddWithValue("@BirthDate", sinhVien.BirthDate);
        cmd.Parameters.AddWithValue("@Gender", sinhVien.Gender);
        cmd.Parameters.AddWithValue("@ClassID", sinhVien.ClassId);
    }
}