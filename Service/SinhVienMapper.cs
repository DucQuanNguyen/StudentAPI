using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using StudentAPI.Service;

public class SinhVienMapper : IDataMapper<SinhVien>
{
    public SinhVien Map(SqlDataReader reader)
    {
        var gender = reader[nameof(SinhVien.Gender)] as string;
        gender = string.IsNullOrWhiteSpace(gender) ? "Other" : gender;

        return new SinhVien
        {
            StudentId = reader[nameof(SinhVien.StudentId)] as string ?? string.Empty,
            StudentName = reader[nameof(SinhVien.StudentName)] as string ?? string.Empty,
            BirthDate = reader[nameof(SinhVien.BirthDate)] is DateTime dt ? dt : Convert.ToDateTime(reader[nameof(SinhVien.BirthDate)]),
            Gender = gender,
            ClassId = reader[nameof(SinhVien.ClassId)] is int id ? id : Convert.ToInt32(reader[nameof(SinhVien.ClassId)])
        };
    }
}