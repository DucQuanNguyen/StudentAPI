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
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required]
    [RegularExpression("Male|Female|Other", ErrorMessage = "Gender must be 'Male', 'Female' or 'Other'.")]
    public string Gender { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ClassId must be a positive integer.")]
    public int ClassId { get; set; }
}
