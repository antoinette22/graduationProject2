using graduationProject.Models;
using System.Text.Json.Serialization;

namespace graduationProject.Dtos
{
    public class ReturnPostsDto
    {
        [JsonIgnore]
        public bool isSucces { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public IEnumerable<ReturnPostDto> Posts { get; set; }
    }
}
