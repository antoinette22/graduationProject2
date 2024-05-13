using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs.AuthDtos
{
    public class UpdateUsernameDto
    {
        [Required, MaxLength(56)]
        public string Username { get; set; }
    }
}
