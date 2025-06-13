using StudentAPI.DTO;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    public static class Mapper
    {
        // Student mapping
        public static StudentDto ToDto(SinhVien s) => new StudentDto
        {
            StudentId = s.StudentId,
            StudentName = s.StudentName,
            BirthDate = s.BirthDate,
            Gender = s.Gender,
            ClassId = s.ClassId
        };

        public static SinhVien ToEntity(StudentDto dto) => new SinhVien
        {
            StudentId = dto.StudentId,
            StudentName = dto.StudentName,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            ClassId = dto.ClassId
        };

        // Class mapping
        public static ClassDto ToDto(LopHoc l) => new ClassDto
        {
            Id = l.Id,
            ClassName = l.ClassName
        };

        public static LopHoc ToEntity(ClassDto dto) => new LopHoc
        {
            Id = dto.Id,
            ClassName = dto.ClassName
        };

        // User mapping
        public static UserDto ToDto(User user) => new UserDto
        {
            UserName = user.UserName,
            Role = user.Role
        };

        public static User ToEntity(UserDto dto) => new User
        {
            UserName = dto.UserName,
            Role = dto.Role
        };

        public static User ToEntity(RegisterUserDto dto) => new User
        {
            UserName = dto.UserName,
            PassWord = dto.PassWord,
            Role = dto.Role
        };
    }
}