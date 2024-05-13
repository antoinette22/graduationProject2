using graduationProject.DTOs;
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
                    Rrice = post.Offers.FirstOrDefault()?.Rrice ?? 0, // handle if Offers are null or empty
                    Description = post.Offers.FirstOrDefault()?.Description,
                    NationalId = post.Offers.FirstOrDefault()?.NationalId,
                    ProfitRate = post.Offers.FirstOrDefault()?.ProfitRate ?? 0 // handle if Offers are null or empty
                };
                response.Add(dto);
            }

            return response;
        }
    }
}
