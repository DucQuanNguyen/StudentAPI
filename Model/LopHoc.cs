﻿using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Model;

public partial class LopHoc
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string ClassName { get; set; }
}
