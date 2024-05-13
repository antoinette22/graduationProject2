namespace graduationProject.Dtos
{
    public class AddPostDto
    {
        public string Content { get; set; }
        public List<IFormFile> Attachments { get; set; } // Change here
        public int CategoryId { get; set; }
    }

}
