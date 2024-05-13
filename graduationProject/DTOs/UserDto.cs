using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? PictureUrl { get; set; }
    }
}
