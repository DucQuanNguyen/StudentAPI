using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Model;

public partial class SinhVien
{
    [Required]
    public string StudentId { get; set; }

    [Required]
    [StringLength(100)]
    public string StudentName { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    [RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
    public string Gender { get; set; }

    [Required]
    public int ClassId { get; set; }
}
