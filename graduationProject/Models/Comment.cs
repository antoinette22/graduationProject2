namespace graduationProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Post Post { get; set; }
        public IEnumerable<Reply> Replies { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Attachment { get; set; }
    }
}
