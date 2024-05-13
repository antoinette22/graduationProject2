using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace graduationProject.Models
{
    public class Chat : BaseEntity
    {
        public string ChatId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "الرساله "), StringLength(int.MaxValue)]
        public string Message { get; set; }

        [ForeignKey("SendUser")]
        public string SendUserId { get; set; }

        public virtual ApplicationUser SendUser { get; set; }

        [ForeignKey("ReceiveUser")]
        public string ReceiveUserId { get; set; }

        public virtual ApplicationUser ReceiveUser { get; set; }

        [NotMapped]
        public List<IFormFile> Attachment { get; set; }

        [NotMapped]
        public List<string> AttachmentUrls { get; set; }

        public string AttachmentUrl { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
