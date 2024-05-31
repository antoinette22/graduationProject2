using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.OffersDtos;

namespace graduationProject.Services
{
    public interface IuserService
    {
        Task<List<searchDto>> SearchUserProfile(string firstName, string lastName);
        Task<List<GetOfferedUserDto>> getOfferedPosts(string id);
        Task<ResultDto> RefuseOffer(int id);
        Task<ResultDto> AcceptOffer(AcceptOfferDto acceptOffer, string username);
    }
}
