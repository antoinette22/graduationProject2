using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs.AuthDtos
{
    public class UpdatePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required, Compare("NewPassword", ErrorMessage = "Confirm Password Doesn't Match New Password")]
        public string ConfirmPassword { get; set; }
    }
}
