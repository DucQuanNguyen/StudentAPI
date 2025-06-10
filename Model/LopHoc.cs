using System;
using System.Collections.Generic;

namespace StudentAPI.Model;

public partial class LopHoc
{
    public int Id { get; set; }

    public string ClassName { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
