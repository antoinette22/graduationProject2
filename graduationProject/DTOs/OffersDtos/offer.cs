using graduationProject.Models;

namespace graduationProject.DTOs.OfferDtos
{
    public class offer
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string NationalcardInvestor { get; set; }
        public double Price { get; set; }
        public double ProfitRate { get; set; }
        public string Description { get; set; }
        public string NationalIdInvestor { get; set; }
        public string? NationalIdUser { get; set; }
        public string? NationalcardUser{ get; set; }
        public string? SignatureUser { get; set; }


    }
}
