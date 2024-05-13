using graduationProject.DTOs.OfferDtos;
using Microsoft.AspNetCore.Identity;

namespace graduationProject.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ApplicationUser User { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<React> Reacts { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        //public string? Attachment { get; set; }
        //public List<string> Attachments { get; set; } = new List<string>();
        public bool IsHaveOffer { get; set; } = false;
        public IEnumerable<offer> Offers { get; set; }
        public ICollection<Attachment> Attachments { get; set; }

    }
}
