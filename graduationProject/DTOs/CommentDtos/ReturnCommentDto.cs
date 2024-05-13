using graduationProject.Models;
using System.Text.Json.Serialization;

namespace graduationProject.Dtos
{
    public class ReturnCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public bool isSucces { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public string? Attachment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
