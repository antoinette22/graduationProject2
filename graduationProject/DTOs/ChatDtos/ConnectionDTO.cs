using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs.ChatDtos
{
    public class ConnectionDTO
    {
        [Required(ErrorMessage = "اسم الصديق مطلوب ")]
        [Display(Name = "اسم الصديق ")]
        public string TargetUserId { get; set; }

        public bool Agree { get; set; }
    }
}
