using graduationProject.DTOs;
using graduationProject.DTOs.OfferDtos;
using graduationProject.Models;

namespace graduationProject.Mapping
{
    public static class GetOfferedUserDtoMap
    {
        public static async Task<List<GetOfferedUserDto>> MapToGetOfferedUserDtoMap(this IEnumerable<Post> posts)
        {
            var response = new List<GetOfferedUserDto>();

            foreach (var post in posts)
            {
                var dto = new GetOfferedUserDto()
                {
                    Id = post.Id,
                    Content = post.Content,
                    //Rrice = post.Offers.FirstOrDefault()?.Price ?? 0, // handle if Offers are null or empty
                    //Description = post.Offers.FirstOrDefault()?.Description,
                    //NationalId = post.Offers.FirstOrDefault()?.NationalIdInvestor,
                    //ProfitRate = post.Offers.FirstOrDefault()?.ProfitRate ?? 0 // handle if Offers are null or empty
                    Offers = post.Offers.Select(offer => new offerDto
                    {
                        Price = offer.Price,
                        Description = offer.Description,
                        NationalId = offer.NationalIdInvestor,
                        ProfitRate = offer.ProfitRate
                    }).ToList()
                };
                response.Add(dto);
            }

            return response;
        }
    }
}
