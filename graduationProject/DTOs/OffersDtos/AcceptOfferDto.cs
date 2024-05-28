namespace graduationProject.DTOs.OffersDtos
{
    public class AcceptOfferDto
    {

        public int offertId { get; set; }
        public IFormFile? Image { get; set; }
        public string NationalId { get; set; }
        public string SignatureUser { get; set; }
    }
}
