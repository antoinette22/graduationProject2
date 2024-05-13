using graduationProject.core.DbContext;
using graduationProject.DTOs;
using graduationProject.Helpers;
using graduationProject.Models;
using graduationProject.Services;
using Investor.Core.DTO.EntityDTO;

//using Investor.BusinessLayer.Interfaces;
//using Investor.Core.DTO;
//using Investor.Core.DTO.EntityDTO;
//using Investor.Core.Entity.ApplicationData;
//using Investor.Core.Entity.ChatandUserConnection;
//using Investor.Core.Entity.PostData;
//using Investor.Core.Helpers;
//using Investor.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Net.Mail;

namespace Investor.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase, IActionFilter
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private ApplicationUser _user;

        public ChatController(/*IUnitOfWork unitOfWork,*/ IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            //_unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _baseResponse = new BaseResponse();
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
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
        //--------------------------------------------------------------------------------------------------------Send Message
        [HttpPost("SendMessage")]
        public async Task<ActionResult<BaseResponse>> SendMessage([FromHeader] string lang, [FromForm] ChatDTO chatDTO)
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

            var ReceiveUser = await _userManager.Users.Where(s => s.Id == chatDTO.ReceiveUserId && s.Status == true).FirstOrDefaultAsync();
            if (ReceiveUser == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " الحساب الذي تراسله غير موجود "
                    : "The Receive User Not Exist ";
                return Ok(_baseResponse);
            }

            if (chatDTO.Attachment != null)
                try
                {
                    foreach (var ChatImg in chatDTO.Attachment)
                    {
                        string img = await _fileHandling.UploadFile(ChatImg, "Chat");
                        chatDTO.AttachmentUrls.Add(img);
                    }
                }
                catch
                {
                    _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                    _baseResponse.ErrorMessage = lang == "ar"
                        ? "خطأ في رفع الملفات "
                        : "Error In Upload Attachments ";
                    return Ok(_baseResponse);
                }

            var Chat = new Chat
            {
                Message = chatDTO.Message,
                SendUserId = _user.Id,
                ReceiveUserId = ReceiveUser.Id,
                AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : null,
                IsRead = false
            };
            await _context.Chats.AddAsync(Chat);
            try
            {
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في ارسال الرساله "
                    : "Error In send message ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم ارسال الرساله بنجاح"
                : "The message Has Been Send Successfully";

            return Ok(_baseResponse);

        }

        //--------------------------------------------------------------------------------------------------------Edit Message
        [HttpPut("EditMessage")]
        public async Task<ActionResult<BaseResponse>> EditMessage([FromHeader] string lang, [FromForm] ChatDTO chatDTO)
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

            var Message = await _context.Chats.Where(s => s.ChatId == chatDTO.ChatId && s.IsDeleted == false).FirstOrDefaultAsync();
            if (Message == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذه الرساله غير موجوده"
                    : "Message Not Exits ";
                return Ok(_baseResponse);

            }

            if (Message.SendUserId != _user.Id)
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "لا يمكنك تعديل هذه الرساله "
                    : "Message Not Exits ";
                return Ok(_baseResponse);

            }

            if (chatDTO.Attachment != null)
                try
                {
                    foreach (var ChatImg in chatDTO.Attachment)
                    {
                        string img = await _fileHandling.UploadFile(ChatImg, "Chat");
                        chatDTO.AttachmentUrls.Add(img);
                    }
                }
                catch
                {
                    _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                    _baseResponse.ErrorMessage = lang == "ar"
                        ? "خطأ في رفع الملفات "
                        : "Error In Upload Attachments ";
                    return Ok(_baseResponse);
                }

            Message.Message = (chatDTO.Message != null) ? chatDTO.Message : Message.Message;
            Message.SendUserId = _user.Id;
            Message.ReceiveUserId = Message.ReceiveUserId;
            Message.AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : Message.AttachmentUrl;
            Message.IsUpdated = true;
            Message.UpdatedAt = DateTime.Now;
            try
            {
                _context.Chats.Update(Message);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في تعديل الرساله "
                    : "Error In Edit message ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم تعديل الرساله بنجاح"
                : "The message Has Been Edit Successfully";

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------Get Message
        [HttpGet("GetMessage")]
        public async Task<ActionResult<BaseResponse>> GetMessage([FromHeader] string lang, [FromHeader] string ReceiveId)
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

            var ReceiveUser = await _userManager.Users.Where(s => s.Id == ReceiveId).FirstOrDefaultAsync();
            if(ReceiveUser == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Receive Not Exist ";
                return Ok(_baseResponse);
            }

            var Messages = await _context.Chats.Where(s => (s.SendUserId == _user.Id && s.ReceiveUserId == ReceiveId) || (s.ReceiveUserId == _user.Id && s.SendUserId == ReceiveId)).Select(x => new
            {
                x.ChatId,
                x.Message,
                SenderUser = new
                {
                    x.SendUser.Id,
                    x.SendUser.FirstName, 
                    x.SendUser.LastName,
                    x.SendUser.Email,
                },
                ReceiveUser = new
                {
                    x.ReceiveUser.Id,
                    x.ReceiveUser.FirstName,
                    x.ReceiveUser.LastName,
                    x.ReceiveUser.Email,
                },
                SendTime = (x.IsUpdated) ? x.UpdatedAt : x.CreatedAt,
                x.IsRead,
                x.IsDeleted,
                x.IsUpdated,
                Attachment=ConvertStringToList(x.AttachmentUrl)
            }).ToListAsync();
            var ReadMessages = await _context.Chats.Where(s => (s.SendUserId == _user.Id && s.ReceiveUserId == ReceiveId)).ToListAsync();

            foreach (var message in ReadMessages)
            {
                message.IsRead = true;
                try
                {
                    _context.Chats.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                    _baseResponse.ErrorMessage = lang == "ar"
                        ? "خطأ في قراءة الرساله "
                        : "Error In Read message ";
                    return Ok(_baseResponse);
                }

            }
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Messages;
            return Ok(_baseResponse);

        }

        //--------------------------------------------------------------------------------------------------------Delete Message
        [HttpDelete("DeleteMessage")]
        public async Task<ActionResult<BaseResponse>> DeleteMessage([FromHeader] string lang, [FromForm] string MessageId)
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

            var Message = await _context.Chats.Where(s => s.ChatId == MessageId && s.IsDeleted == false).FirstOrDefaultAsync();
            if (Message == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذه الرساله غير موجوده"
                    : "Message Not Exits ";
                return Ok(_baseResponse);

            }

            if (Message.SendUserId != _user.Id)
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "لا يمكنك تعديل هذه الرساله "
                    : "Message Not Exits ";
                return Ok(_baseResponse);

            }

            Message.IsDeleted = true;
            Message.DeletedAt = DateTime.Now;
            try
            {
                _context.Chats.Update(Message);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في حذف الرساله "
                    : "Error In Delete Message ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم حذف الرساله بنجاح"
                : "The Message Has Been Deleted Successfully";

            return Ok(_baseResponse);

        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Function
        static string ConvertListToString(List<string> list)
        {
            return string.Join(",", list);
        }

        static List<string> ConvertStringToList(string inputString)
        {
            // Split the input string by commas and convert it to a List<string>
            return inputString.Split(',').ToList();
        }

    }
}
