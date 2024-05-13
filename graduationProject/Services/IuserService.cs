using graduationProject.Dtos;
using graduationProject.DTOs;

namespace graduationProject.Services
{
    public interface IuserService
    {
        Task<searchDto> SearchUserProfile(string userName);
        Task<List<GetOfferedUserDto>> getOfferedPosts(string id);
        Task<ResultDto> RefuseOffer(int id);
    }
}
