using graduationProject.Dtos;

namespace graduationProject.Services
{
    public interface IAccountService
    {
        Task<ResultDto> DeleteUserAccount(string id);
    }
}
