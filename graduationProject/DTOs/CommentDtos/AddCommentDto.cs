namespace graduationProject.Dtos
{
    public class AddCommentDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public IFormFile? Attachment { get; set; }
    }
}
