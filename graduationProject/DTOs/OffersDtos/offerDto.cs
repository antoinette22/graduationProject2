using graduationProject.Models;

namespace graduationProject.DTOs.OfferDtos
{
    public class offerDto
    {
        public int postId { get; set; }
        public IFormFile? Image { get; set; }
        public double Price { get; set; }
        public double ProfitRate { get; set; }
        public string Description { get; set; }
        public string NationalId { get; set; }
      
    }
}
