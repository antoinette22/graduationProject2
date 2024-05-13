using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using graduationProject.Helpers;

namespace Investor.Core.DTO.EntityDTO
{
    public class ChatDTO
    {
        public string ChatId { get; set; }

        [Display(Name = "الرساله "), StringLength(int.MaxValue)]
        [ExclusiveField("Attachment", ErrorMessage = "Either Message or Attachment should be entered.")]
        public string Message { get; set; }

        [Required]
        [Display(Name ="المرسل اليه"),StringLength(int.MaxValue)]
        public string ReceiveUserId { get; set; }

        [Display(Name = " الملفات  ")]
        [ExclusiveField("Message", ErrorMessage = "Either Message or Attachment should be entered.")]
        public List<IFormFile> Attachment { get; set; }

        public List<string> AttachmentUrls { get; set; } = new List<string>();

    }
}
