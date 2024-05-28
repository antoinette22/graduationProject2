using graduationProject.core.DbContext;
using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.OfferDtos;
using graduationProject.DTOs.OffersDtos;
using graduationProject.Mapping;
using graduationProject.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace graduationProject.Services
{
    public class userService : IuserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long maxAllowedSize = 1048576;
        private readonly string _uploadsPath;
        public userService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _uploadsPath = Path.Combine(env.WebRootPath, "cardId");
        }


        public async Task<List<GetOfferedUserDto>> getOfferedPosts(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Posts)
                .ThenInclude(p => p.Offers)
                .FirstOrDefaultAsync(x => x.Id == id);
            //FirstOrDefaultAsync
            if (user == null || user.Posts == null)
                return null; // or handle the situation as appropriate

            // Filter the posts that have offers
            var postsWithOffers = user.Posts.Where(p => p.IsHaveOffer);

            var response = await postsWithOffers.MapToGetOfferedUserDtoMap();
            return response;
        }

        public async Task<ResultDto> RefuseOffer(int id)
        {
            //var offer = await _context.Offers.FindAsync(id);
            var offer = await _context.Offers
                .Include(o => o.Post)       // Include the related post
                .ThenInclude(p => p.User)   // Include the user related to the post
                .FirstOrDefaultAsync(o => o.Id == id);
            if (offer == null)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = "offer not found"
                };
            }

            //   var post = _context.Posts.FirstOrDefault(o => o.Id==offer.PostId);
            offer.Post.IsHaveOffer = false;
            _context.Offers.Remove(offer);
            _context.Posts.Update(offer.Post);
            _context.SaveChanges();

            return new ResultDto()
            {
                IsSuccess = true,
                Message = "offer removed Successfuly"
            };


        }

        public async Task<ResultDto> AcceptOffer(AcceptOfferDto acceptOffer)
        {
            var offer = await _context.Offers.FindAsync(acceptOffer.offertId);

            if (offer == null)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = "offer not found"
                };
            }
            if (acceptOffer.Image != null)
            {
                var extension = Path.GetExtension(acceptOffer.Image.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ResultDto()
                    {
                        IsSuccess = false,
                        Message = "image not found"
                    };
                }
                if (acceptOffer.Image.Length > maxAllowedSize)
                {
                    return new ResultDto()
                    {
                        IsSuccess = false,
                        Message = "image is too large"
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await acceptOffer.Image.CopyToAsync(fileStream);
                }

                offer.NationalcardUser = fileName;
            }

                offer.NationalIdUser = acceptOffer.NationalId;
                offer.SignatureUser = acceptOffer.SignatureUser;

                // Save the changes to the database
                _context.Offers.Update(offer);
                await _context.SaveChangesAsync();

                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "Offer accepted successfully"
                };
        }


            public async Task<searchDto> SearchUserProfile(string userName)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName.Contains(userName.Trim()));
                if (user != null)
                {
                    return new searchDto

                    {
                        userName = user.UserName,
                    };
                }
                else return new searchDto

                {
                    userName = null,
                };

            }
        }
}

