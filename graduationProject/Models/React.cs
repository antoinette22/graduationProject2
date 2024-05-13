using Microsoft.AspNetCore.Identity;

namespace graduationProject.Models
{
    public class React
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
    }
}
