using graduationProject.core.DbContext;
using graduationProject.DTOs;
using graduationProject.Models;
using graduationProject.Services;
using Investor.Core.DTO.EntityDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Parameters;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace graduationProject.Helpers
{
    public class ChatHub : Hub
    {
        //private readonly IUnitOfWork _context;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatHub(/*IUnitOfWork unitOfWork,*/ IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            //_context = unitOfWork;
            _fileHandling = fileHandling;
            _baseResponse = new BaseResponse();
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// check model is null or not and if you send message or image
        /// check token and Received user 
        /// check if 2 user have connection or no 
        /// get all message between 2 user
        /// edit all message Received user send to you be read 
        /// save message in database
        /// send message to Received user
        /// </summary>
        /// <returns>all message between 2 user </returns>
        public async Task SendMessage(ChatDTO chatDTO)
        {
            try
            {
                if (chatDTO == null || string.IsNullOrEmpty(chatDTO.ReceiveUserId) || (string.IsNullOrEmpty(chatDTO.Message) && chatDTO.Attachment == null))
                {
                    throw new ArgumentException("Message cannot be null or empty.");
                }
                var accessToken = Context.GetHttpContext().Request.Query["access_token"];
                var userId = JWT(accessToken);
                var connection = _context.Connections.Where(s => (s.User1Id == userId && s.User2Id == chatDTO.ReceiveUserId && s.IsAgree) || (s.User1Id == chatDTO.ReceiveUserId && s.User2Id == userId && s.IsAgree)).FirstOrDefault();
                if (connection == null)
                {
                    throw new ArgumentException("User not connection with this user.");
                }
                if (userId == null)
                {
                    throw new ArgumentException("Token is mistake.");
                }
                var user = _userManager.Users.Where(s => s.Id == userId && s.Status)
                .FirstOrDefault();
                var ReceivedUser = _userManager.Users.Where(s => s.Id == chatDTO.ReceiveUserId && s.Status)
                                                .FirstOrDefault();
                if (user == null && ReceivedUser == null)
                {
                    throw new ArgumentException("Not can't found user.");
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
                        await Clients.Group(connection.ConnectionId).SendAsync("Error", "Error in upload image");
                    }
                var ReadMessages = await _context.Chats.Where(s => s.SendUserId == chatDTO.ReceiveUserId && s.ReceiveUserId == user.Id && !s.IsRead).ToListAsync();

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
                        Console.WriteLine("An error occurred: Error in update data");
                        throw;

                    }

                }
                var Chat = new Chat
                {
                    Message = chatDTO.Message,
                    SendUserId = user.Id,
                    ReceiveUserId = ReceivedUser.Id,
                    AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : null,
                    IsRead = false
                };
                try
                {
                    _context.Chats.Add(Chat);
                    await _context.SaveChangesAsync();
                    await Clients.Group(connection.ConnectionId).SendAsync("Message", new
                    {
                        Message = chatDTO.Message,
                        SendUserId = user.Id,
                        ReceiveUserId = ReceivedUser.Id,
                        AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : null,
                        IsRead = false,
                        CreatedAt = Chat.CreatedAt
                    });
                }
                catch
                {
                    await Clients.Group(connection.ConnectionId).SendAsync("Error", "Error In send message");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task EditMessage(ChatDTO chatDTO)
        {
            if (chatDTO == null || string.IsNullOrEmpty(chatDTO.ReceiveUserId) || (string.IsNullOrEmpty(chatDTO.Message) && chatDTO.Attachment == null))
            {
                return;
            }
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            var userId = JWT(accessToken);
            if (userId == null)
            {
                return;
            }
            var connection = _context.Connections.Where(s => (s.User1Id == userId && s.User2Id == chatDTO.ReceiveUserId && s.IsAgree) || (s.User1Id == chatDTO.ReceiveUserId && s.User2Id == userId && s.IsAgree)).FirstOrDefault();
            if (connection != null)
            {
                await Clients.Group(connection.ConnectionId).SendAsync("Error", "Error Can't edit message not found connection");
            }
            var Message = await _context.Chats.Where(s => s.ChatId == chatDTO.ChatId && s.IsDeleted == false).FirstOrDefaultAsync();
            if (Message != null)
            {
                return;
            }

            var user = _context.Users.Where(s => s.Id == userId && s.Status)
                            .FirstOrDefault();
            var ReceivedUser = _context.Users.Where(s => s.Id == chatDTO.ReceiveUserId && s.Status)
                                            .FirstOrDefault();
            if (user != null && ReceivedUser != null)
            {

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
                        await Clients.Group(connection.ConnectionId).SendAsync("Error", "Error in upload image");
                    }
                Message.Message = (chatDTO.Message != null) ? chatDTO.Message : Message.Message;
                Message.SendUserId = user.Id;
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
                    await Clients.Group(connection.ConnectionId).SendAsync("Error", "Error In Edit message");
                }
            }
            await Clients.Group(connection.ConnectionId).SendAsync("MessageEdited", Message);
        }

        public async Task DeleteMessage(string messageId)
        {
            if (messageId == null)
            {
                return;
            }
            var Message = await _context.Chats.Where(s => s.ChatId == messageId && s.IsDeleted == false).FirstOrDefaultAsync();
            Message.IsDeleted = true;
            var connection = _context.Connections.Where(s => (s.User1Id == Message.SendUserId && s.User2Id == Message.ReceiveUserId && s.IsAgree) || (s.User1Id == Message.ReceiveUserId && s.User2Id == Message.SendUserId && s.IsAgree)).FirstOrDefault();

            try
            {
                _context.Chats.Update(Message);
                await _context.SaveChangesAsync();
            }
            catch
            {
                await Clients.User(connection.ConnectionId).SendAsync("Error", "Error In Delete message");
            }

            await Clients.Group(connection.ConnectionId).SendAsync("MessageDeleted", messageId);
        }

        /// <summary>
        /// create connection with server in signalr 
        /// check token and Received user 
        /// check if 2 user have connection or no 
        /// get all message between 2 user
        /// edit all message Received user send to you be read 
        /// </summary>
        /// <returns>all message between 2 user </returns>
        public override async Task OnConnectedAsync()
        {
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            var ReceivedId = Context.GetHttpContext().Request.Query["ReceivedId"].ToString();
            var userId = JWT(accessToken);
            var connection = _context.Connections.Where(s => (s.User1Id == userId && s.User2Id == ReceivedId && s.IsAgree) || (s.User1Id == ReceivedId && s.User2Id == userId && s.IsAgree)).FirstOrDefault();
            try
            {
                if (connection == null)
                {
                    throw new ArgumentException("Error You Can't connection");
                }
                if (userId == null && ReceivedId == null)
                {
                    throw new ArgumentException("Not can't found user.");
                }
                var user = _context.Users.Where(s => s.Id == userId && s.Status)
                                                .FirstOrDefault();
                var ReceivedUser = _context.Users.Where(s => s.Id == ReceivedId && s.Status)
                                                .FirstOrDefault();
                var ReadMessages = await _context.Chats.Where(s => s.SendUserId == ReceivedId && s.ReceiveUserId == user.Id && !s.IsRead).ToListAsync();

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
                        Console.WriteLine("An error occurred: Error in update data");
                        throw;

                    }

                }
                await Groups.AddToGroupAsync(Context.ConnectionId, connection.ConnectionId);
                await base.OnConnectedAsync();
                var Messages = await _context.Chats.Where(s => (s.SendUserId == userId && s.ReceiveUserId == ReceivedId) || (s.ReceiveUserId == userId && s.SendUserId == ReceivedId)).Select(x => new
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
                    Attachment = (x.AttachmentUrl == null) ? null : ConvertStringToList(x.AttachmentUrl)
                }).ToListAsync();
                foreach (var message in Messages)
                {
                    await Clients.Group(connection.ConnectionId).SendAsync("Message", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;

            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        //--------------------------------------------------------------------------------------

        public string JWT(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                // Invalid token format
                return null;
            }

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid").Value;
            return userId;
        }

        static List<string> ConvertStringToList(string inputString)
        {
            // Split the input string by commas and convert it to a List<string>
            return inputString.Split(',').ToList();
        }

        static string ConvertListToString(List<string> list)
        {
            return string.Join(",", list);
        }


    }
}
