using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs.AuthDtos
{
    public class EmailDto
    {
        [Required]
        public string Email { get; set; }
    }
}
