using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace graduationProject.Models
{
    public class Connection
    {
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("User1")]
        [Display(Name = "اسم المستخدم 1 ")]
        public string User1Id { get; set; }

        [Display(Name = "اسم المستخدم 1 ")]
        public virtual ApplicationUser User1 { get; set; }

        public bool IsAgree { get; set; } = false;

        [ForeignKey("User2")]
        [Required, Display(Name = "اسم المستخدم 2 ")]
        public string User2Id { get; set; }

        [Display(Name = "اسم المستخدم 2 ")]
        public virtual ApplicationUser User2 { get; set; }

    }
}
