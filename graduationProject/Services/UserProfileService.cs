
using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.AuthDtos;
using graduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace graduationProject.Services
{
    public class UserProfileService: IUserProfileService
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailingService _mailingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        public UserProfileService(IAuthService authService, IMailingService mailingService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, UserManager<ApplicationUser> userManager)
        {

            _authService = authService;
            _mailingService = mailingService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _userManager = userManager;
        }


        /*  public async Task UpdatePasswordAsync(string email, UpdatePasswordDto dto)
          {
              var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
              if (user == null)
              {
                  return new ResultDto
                  {
                      IsSuccess = false,
                      Message = "User not found"
                  };
              }
              var result = await _unitOfWork.UserManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
              if (result.Succeeded)
              {
                  await _authService.RevokeToken(user.RefreshTokens.FirstOrDefault(r => r.IsActive).Token);
                  return new ResultDto
                  {
                      IsSuccess = true,
                      Message = "Password updated successfully"
                  };
              }
              return new ResultDto
              {
                  IsSuccess = false,
                  Message = "Password update failed"
              };
          }*/
        public async Task<ResultDto> UpdateEmailAsync(string email, string Newemail)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            if (user.Email == Newemail)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Email is same as current email"
                };
            }
            if(_userManager.Users.Any(u => u.Email == Newemail))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Email already exists"
                };
            }

            var token =await _userManager.GenerateChangeEmailTokenAsync(user, Newemail);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = Convert.ToBase64String(encodedToken);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var callBackUrl = urlHelper.Action("ConfirmNewEmail", "Auth", new { Id = user.Id,NewEmail=Newemail,Token = validToken }, _httpContextAccessor.HttpContext.Request.Scheme);
            var message = new MailRequestDto
            {
                MailTo = Newemail,
                Subject = "Confirm Your New Email",
                Content = $"<h1>Welcome to GP</h1> <p>Please confirm your email by <a href='{callBackUrl}'>Clicking here</a></p>",
            };
            try
            {
                var mailResult = await _mailingService.SendEmailAsync(message.MailTo, message.Subject, message.Content);
                return new ResultDto { IsSuccess = true, Message = "Confirmation Email Was Sent, Please confirm your new email" };
            }
            catch
            {
                return new ResultDto { Message = "Confirmation Email Failed to send" };
            }

        }
        public async Task<ResultDto> UpdateUsername(string email, string username)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "User not found" }
                };
            }
            if (user.UserName == username)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "Username is the same as the current username" }
                };
            }
            if (_userManager.Users.Any(u => u.UserName == username))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "Username already used" }
                };
            }
            user.UserName = username;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Username updated successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Something wrong, username was not updated" }
            };
        }
        public async Task<ResultDto> UpdatePasswordAsync(string email, UpdatePasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "User not found" }
                };
            }
            if (!await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "Current password is not correct" }
                };
            }
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                //await _authService.RevokeToken(user.RefreshTokens.FirstOrDefault(r => r.IsActive).Token);
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Password updated successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Something wrong, password was not updated" }
            };
        }
    }
}
