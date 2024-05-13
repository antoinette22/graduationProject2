using Azure.Identity;
using graduationProject.core.DbContext;
using graduationProject.core.Dtos;
using graduationProject.core.OtherObjects;
using graduationProject.DTOs.AuthDtos;
using graduationProject.Models;
using graduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace graduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context, IAuthService authService, IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _authService = authService;
            _userProfileService = userProfileService;
        }
        //seed roles
        /* [HttpPost]
         [Route("seed-roles")]
         public async Task<IActionResult> SeedRoles()
         {
             bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
             bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
             bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
             if(isOwnerRoleExists && isAdminRoleExists && isUserRoleExists)
                 return Ok("Roles Seeding is already Done");
             await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
             await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
             await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
             return Ok("Roles Seeding Done Successfully");

         }
        */
        // register
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
                return BadRequest(result.Message);

            //if (result.ErrorMessage is not null)
            //return BadRequest(result);
            //return Ok(new {result.Token , result.ExpiresOn});
            //setrefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
         /* [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser != null)
                return BadRequest("UserName already Exist");
            var user = await _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
                return BadRequest("Email already Exist");

            IdentityUser newUser = new IdentityUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),

            };
            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation failed because: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += "#" + error.Description;
                }
                return BadRequest(errorString);
            }
            if (registerDto.IsInvestor)
                await _userManager.AddToRoleAsync(newUser, "Investor");
            else
                await _userManager.AddToRoleAsync(newUser, "User");
            return Ok("User Created Successfully");

        }*/
        // Login 
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                return BadRequest("Invalid Credentials");
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return BadRequest("Invalid Credentials");
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("UserId", user.Id),
                new Claim("JWTID",Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName)
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            if (!user.EmailConfirmed)
            {
                return BadRequest("Please confirm your email .");
            }
            var token = GenerateNewJsonWebToken(authClaims);
            return Ok(new { Message = "Login successful.", Token = token });

        }
        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)

                );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;

        }
        [Authorize]
        [HttpPut("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdateUsername(email, dto.Username);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Message);
        }

        /* [HttpPost]
         [Route("change UserEmail")]
         public async Task<IActionResult> changeEmail([FromBody] ChangeUserEmailDto changeUserEmail)
         {
             try
             {
                 if (string.IsNullOrEmpty(changeUserEmail.UserName))
                 {
                     return BadRequest("UserName is required");
                 }

                 // var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == changeUserName.UserName);
                 var user = await _userManager.FindByNameAsync(changeUserEmail.UserName);
                 if (user == null)
                 {
                     return NotFound("User not found");
                 }

                 if (!string.IsNullOrEmpty(changeUserEmail.newEmail))
                 {
                     user.Email = changeUserEmail.newEmail;
                 }

                 _context.Entry(user).State = EntityState.Modified;
                 await _context.SaveChangesAsync();

                 return Ok("User email changed successfully");
             }
             catch (Exception ex)
             {
                 // Handle exceptions appropriately
                 return StatusCode(500, $"Internal server error: {ex.Message}");
             }

         }
        */
        [Authorize]
        [HttpPut("ChangeEmail")]
        public async Task<IActionResult> UpdateEmail([FromForm] string NewEmail)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdateEmailAsync(email, NewEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdatePasswordAsync(email, dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Message);
        }
        //try
        //{
        //    if (string.IsNullOrEmpty(changePassword.UserName))
        //    {
        //        return BadRequest("UserName is required");
        //    }

        //    // var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == changeUserName.UserName);
        //    var user = await _userManager.FindByNameAsync(changePassword.UserName);
        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    if (!string.IsNullOrEmpty(changePassword.newEmail))
        //    {
        //        user.Email = changePassword.newEmail;
        //    }

        //    _context.Entry(user).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return Ok("User information updated successfully");
        //}
        //catch (Exception ex)
        //{
        //    // Handle exceptions appropriately
        //    return StatusCode(500, $"Internal server error: {ex.Message}");
        //}
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return NotFound("Invalid Email Confirmation Url");
            var user = await _userManager.FindByIdAsync(userId);
            // code = Encoding.UTF8.GetString(Convert.FromBase64String(code));
            var decodedToken = WebEncoders.Base64UrlDecode(code);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (!result.Succeeded)
                return BadRequest("Email Confirmation Failed");
            return Ok("Email Confirmed Succesfully");


        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [HttpGet("ConfirmNewEmail")]
        public async Task<IActionResult> ConfirmNewEmail(string Id, string NewEmail, string Token)
        {
            var result = await _authService.ConfirmNewEmailAsync(Id, NewEmail, Token);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("verify-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto codeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.VerifyCodeAsync(codeDto);
            if (result.IsSuccess)
            {
                return Ok(result);

            }
            var errors = new { errors = result.Errors };
            return BadRequest(errors);
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ResetPasswordAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Errors);
        }
    }

}