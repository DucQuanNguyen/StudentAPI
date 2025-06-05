using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class LopHoc
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public List<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

    }
}
