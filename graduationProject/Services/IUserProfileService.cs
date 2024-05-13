

using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.AuthDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graduationProject.Services
{
    public interface IUserProfileService
    {
        //  Task<ResultDto> UpdateNameAsync(string email, UpdateNameDto dto);
        Task<ResultDto> UpdateEmailAsync(string userName, string newEmail);
        //  Task UpdatePasswordAsync(string email, UpdatePasswordDto dto);
        Task<ResultDto> UpdatePasswordAsync(string email, UpdatePasswordDto dto);
        Task<ResultDto> UpdateUsername(string email, string username);

    }
}
