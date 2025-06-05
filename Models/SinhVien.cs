using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class SinhVien
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Gender { get; set; }
        public DateTime DOB { get; set; }
        public int ClassId { get; set; }

        public LopHoc LopHoc { get; set; }
    }
}
