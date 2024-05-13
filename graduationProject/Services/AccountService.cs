using graduationProject.core.DbContext;
using graduationProject.Dtos;
using graduationProject.Models;
using Microsoft.AspNetCore.Identity;

namespace graduationProject.Services
{
    public class AccountService:IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ResultDto> DeleteUserAccount(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = new string[] {"User not found"}
                };
            }
            await _userManager.DeleteAsync(user);
            return new ResultDto
            {
                IsSuccess = true,
                Message= "Account deleted successfully"
            };
        }
    }
}
