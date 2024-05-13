using System.Text.Json.Serialization;

namespace graduationProject.DTOs.CategoryDtos
{
    public class ReturnCategoryDto:CategoryDto
    {
        [JsonIgnore]
        public bool isSucces { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
    }
}
