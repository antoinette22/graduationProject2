using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.AuthDtos;
using graduationProject.Models;

namespace graduationProject.Services
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterModel model);
        Task<ResultDto> ConfirmNewEmailAsync(string Id, string newEmail, string Token);
        Task<ResultDto> ResetPasswordAsync(ResetPasswordDto model);
        Task<ResetTokenDto> VerifyCodeAsync(VerifyCodeDto codeDto);
        Task<ResultDto> ForgotPasswordAsync(string email);
    }
}
