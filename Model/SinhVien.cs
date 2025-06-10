using System;
using System.Collections.Generic;

namespace StudentAPI.Model;

public partial class SinhVien
{
    public string StudentId { get; set; } = null!;

    public string StudentName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string? Gender { get; set; }

    public int? ClassId { get; set; }

    public virtual LopHoc? Class { get; set; }
}
