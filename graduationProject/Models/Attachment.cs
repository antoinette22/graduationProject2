namespace graduationProject.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
