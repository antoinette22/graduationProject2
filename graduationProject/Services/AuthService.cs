using graduationProject.core.DbContext;
using graduationProject.core.Dtos;
using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.AuthDtos;
using graduationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace graduationProject.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        //private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailingService _mailingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public AuthService(UserManager<ApplicationUser> userManager,/* IOptions<JWT> jwt, */RoleManager<IdentityRole> roleManager, IMailingService mailingService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, ApplicationDbContext context)
        {
            _userManager = userManager;
            //  _jwt = jwt.Value;
            _roleManager = roleManager;
            _mailingService = mailingService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _context = context;
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new RegisterResultDto { Message = "The Email Is Already Registered" };
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new RegisterResultDto { Message = "The UserName Is Already Registerd" };
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new RegisterResultDto { Message = errors };
            }
            if (model.IsInvestor)
                await _userManager.AddToRoleAsync(user, "Investor");
            else
                await _userManager.AddToRoleAsync(user, "User");
            var Code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(Code);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var link = urlHelper.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = validEmailToken }, _httpContextAccessor.HttpContext.Request.Scheme);
            //var callbackUrl = $"https://localhost:7298/api/auth/Confirmemail?userId={user.Id}&code={Code}";
            //var callbackUrl = _httpContextAccessor.HttpContext.Request.Scheme+"://"+_httpContextAccessor.HttpContext.Request.Host+urlHelper.Action("ConfirmEmail", "AuthController", new { userId = user.Id, code = Code });
            var message = new MailRequestDto
            {
                MailTo = user.Email,
                Subject = "Confirm Your Email",
                Content = $"Please confirm your email by clicking this link: <a href='{link}'>link</a>",
            };
            try
            {
                var mailResult = await _mailingService.SendEmailAsync(message.MailTo, message.Subject, message.Content);
                if (mailResult)
                {
                    return new RegisterResultDto { Message = "Please verify your email ,through the verification email we have just sent", Success = true };
                }
            }
            catch (System.Exception ex)
            {
                //await _userManager.DeleteAsync(user);
                return new RegisterResultDto { Message = "Something went wrong , verification email was not sent", Success = false };
            }
            //await _userManager.DeleteAsync(user);
            return new RegisterResultDto { Message = "Something went wrong", Success = false };

            //await _userManager.AddToRoleAsync(user, "Moderator");

            //var jwtSecurityToken = await CreateToken(user);
            //var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //var refreshToken = GenerateRefreshToken();
            //user.RefreshTokens.Add(refreshToken);
            //await _userManager.UpdateAsync(user);
            //return new AuthModel
            //{
            //    IsAuthenticated = true,
            //    Email = user.Email,
            //    UserName = user.UserName,
            //    Token = token,
            //    //ExpiresOn = jwtSecurityToken.ValidTo, // this is not needed because we are using refresh token expiration
            //    Roles = jwtSecurityToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList(),
            //    RefreshTokenExpiration = refreshToken.ExpiresOn,
            //    RefreshToken = refreshToken.Token

            //};

        }
        public async Task<ResultDto> ConfirmNewEmailAsync(string Id, string newEmail, string Token)
        {
            
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Token))
              {
               return new ResultDto { IsSuccess = false ,Message= "Invalid Email Confirmation Url"
            
               };  
            }
            var user = await _userManager.FindByIdAsync(Id);
            var decodedToken = WebEncoders.Base64UrlDecode(Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ChangeEmailAsync(user, newEmail, normalToken);
            if (!result.Succeeded)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Invalid Email Confirmation process"

                };
            }
            return new ResultDto
            {

                IsSuccess = true,
                Message = "Email Confirmed Successfully"
            };
           
        }
        public async Task<ResultDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Email is incorrect or not found" }
            };
            if (!user.EmailConfirmed) return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Email is not confirmed" }
            };

            Random rnd = new Random();
            var randomNum = (rnd.Next(100000, 999999)).ToString();
            string message = "Hi " + user.UserName + " Your password verification code is: " + randomNum;
            var result = await _mailingService.SendEmailAsync(user.Email, "Password Reset Code ", message, null);
            if (result)
            {
                var Vcode = new VerificationCode
                {
                    Code = randomNum,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                };
                await _context.VerificationCodes.AddAsync(Vcode);
                await _context.SaveChangesAsync();
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Verify code was sent to the email successfully !!",
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Invalid Email" }
            };
        }
        public async Task<ResetTokenDto> VerifyCodeAsync(VerifyCodeDto codeDto)
        {
            var user = await _userManager.FindByEmailAsync(codeDto.Email);
            if (user == null)
            {
                return new ResetTokenDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "Email is incorrect or not found" }
                };
            };
            var result = await _context.VerificationCodes.FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == codeDto.Code);


            if (result != null && !result.IsExpired)
            {
                _context.VerificationCodes.Remove(result);
                await _context.SaveChangesAsync();

                var restToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                return new ResetTokenDto
                {
                    IsSuccess = true,
                    Message = "Code Was Verified Successfully",
                    Token = restToken,
                    Email = user.Email
                };
            }
            return new ResetTokenDto
            {
                IsSuccess = false,
                Errors = new string[] { "Verification code is not correct" }
            };
        }
        public async Task<ResultDto> ResetPasswordAsync(ResetPasswordDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var res = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (res.Succeeded)
                {
                    return new ResultDto
                    {
                        IsSuccess = true,
                        Message = "Password Changed Successfully"
                    };
                }
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] { "Something wrong, password was not changed" }
                };

            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = new string[] { "Something wrong, password was not changed" }
            };


        }
    }
}
