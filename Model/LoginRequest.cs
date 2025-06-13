using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Model
{
    public class LoginRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string PassWord { get; set; } = null!;
    }
}
