using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs.AuthDtos
{
    public class VerifyCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        //[Required]
        //public string Purpose { get; set; }
    }
}
