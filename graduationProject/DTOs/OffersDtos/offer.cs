using graduationProject.Models;

namespace graduationProject.DTOs.OfferDtos
{
    public class offer
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Image { get; set; }
        public double Rrice { get; set; }
        public double ProfitRate { get; set; }
        public string Description { get; set; }
        public string NationalId { get; set; }

    }
}
