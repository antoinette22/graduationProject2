using graduationProject.Models;
using System.Text.Json.Serialization;

namespace graduationProject.Dtos
{
    public class ReturnPostDto:PostDto
    {
        //public int Id { get; set; }
        //public string Content { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        //public string? Attachment { get; set; }
        //public List<string>? Attachments { get; set; }
        public List<string> Attachments { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Reacts { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
