namespace graduationProject.Dtos
{
    public class AddReplyDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public IFormFile? Attachment { get; set; }
    }
}
