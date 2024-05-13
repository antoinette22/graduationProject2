using graduationProject.core.DbContext;
using graduationProject.DTOs;
using graduationProject.DTOs.ChatDtos;
using graduationProject.Helpers;
using graduationProject.Models;
using graduationProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace graduationProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : ControllerBase, IActionFilter
    {
        //private readonly IUnitOfWork _context;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConnectionController(/*IUnitOfWork unitOfWork,*/ IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            //_context = unitOfWork;
            _fileHandling = fileHandling;
            _baseResponse = new BaseResponse();
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(accessToken))
                return;

            var userId = User.Claims.First(i => i.Type == "UserId").Value; // will give the user's userId
            var user = _userManager.Users.Where(s => s.Id == userId && s.Status == true)
                .FirstOrDefault();
            _user = user;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        //--------------------------------------------------------------------------------------------------------
        // Add Request Connection
        [HttpPost("AddConnection")]
        public async Task<ActionResult<BaseResponse>> AddConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }
            if (_user.Id == connectionDTO.TargetUserId)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "لا يمكن ارسال طلب صداقه لنفسك" : "Can't send connection to yourself";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                return Ok(_baseResponse);
            }
            var Connection = await _context.Connections.Where(s => (s.User1Id == _user.Id && s.User2Id == connectionDTO.TargetUserId) || (s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId)).FirstOrDefaultAsync();
            if (Connection != null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " تم ارسال طلب الصداقه من قبل "
                    : "The users are send connection ";
                return Ok(_baseResponse);
            }
            var Connect = new Connection
            {
                User1Id = _user.Id,
                User2Id = connectionDTO.TargetUserId,
                IsAgree = false
            };
            await _context.Connections.AddAsync(Connect);
            await _context.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم ارسال طلب الصداقه بنجاح"
                : "The user send request connection Successfully";

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Accept or Rejection Request Connection
        [HttpPut("AcceptConnection")]
        public async Task<ActionResult<BaseResponse>> AcceptConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var Connection = await _context.Connections.Where(s => s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId).FirstOrDefaultAsync();
            if (Connection == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " لم يتم ارسال طلب الصداقه من قبل "
                    : "The user isn't send connection ";
                return Ok(_baseResponse);
            }

            if (connectionDTO.Agree == true)
            {
                Connection.IsAgree = true;

                _context.Connections.Update(Connection);
                await _context.SaveChangesAsync();

                _baseResponse.ErrorCode = (int)Errors.Success;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "تم قبول طلب الصداقه بنجاح"
                    : "The Connection Has Been Accepted Successfully";

                return Ok(_baseResponse);
            }
            else
            {
                _context.Connections.Remove(Connection);
                await _context.SaveChangesAsync();

                _baseResponse.ErrorCode = (int)Errors.Success;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "تم رفض طلب الصداقه بنجاح"
                    : "The Connection Has Been Rejection Successfully";
                return Ok(_baseResponse);
            }


        }

        //--------------------------------------------------------------------------------------------------------
        //  Cancel Connection
        [HttpDelete("CancelConnection")]
        public async Task<ActionResult<BaseResponse>> CancelConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var Connection = await _context.Connections.Where(s => (s.User1Id == _user.Id && s.User2Id == connectionDTO.TargetUserId) || (s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId) && s.IsAgree == true).FirstOrDefaultAsync();
            if (Connection == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " لم يتم ارسال طلب الصداقه من قبل "
                    : "The user isn't send connection ";
                return Ok(_baseResponse);
            }

            _context.Connections.Remove(Connection);
            await _context.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم الغاء طلب الصداقه بنجاح"
                : "The Connection Has Been Cancel Successfully";
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        //  Get all Accept Connection
        [HttpGet("AllAcceptConnection")]
        public async Task<ActionResult<BaseResponse>> GetAllConnection([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Connection = await _context.Connections.Where(s => (s.User1Id == _user.Id || s.User2Id == _user.Id) && s.IsAgree == true).FirstOrDefaultAsync();
            if (Connection == null)
            {
                _baseResponse.ErrorCode = 0;
                _baseResponse.Data = null;
                return Ok(_baseResponse);
            }
            var data = new Connection
            {
                IsAgree = true,
                User1Id = Connection.User1Id,
                User2Id = Connection.User2Id,
            };
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = data;
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        //  Get all Wait response Connection
        [HttpGet("AllWaitConnection")]
        public async Task<ActionResult<BaseResponse>> GetAllWaitConnection([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Connection = await _context.Connections.Where(s => s.User1Id == _user.Id || s.User2Id == _user.Id && s.IsAgree == false).FirstOrDefaultAsync();
            if (Connection == null)
            {
                _baseResponse.ErrorCode = 0;
                _baseResponse.Data = null;
                return Ok(_baseResponse);
            }
            var data = new Connection
            {
                User1Id = Connection.User1Id,
                User2Id = Connection.User2Id,
            };
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = data;
            return Ok(_baseResponse);
        }

    }
}
