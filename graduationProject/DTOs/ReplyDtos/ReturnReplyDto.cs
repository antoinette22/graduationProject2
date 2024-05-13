using System.Text.Json.Serialization;

namespace graduationProject.Dtos
{
    public class ReturnReplyDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public int CommentId { get; set; }
        [JsonIgnore]
        public bool isSucces { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public string? Attachment { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
