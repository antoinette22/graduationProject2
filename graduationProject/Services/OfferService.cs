using graduationProject.core.DbContext;
using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.OfferDtos;
using graduationProject.Models;
using Microsoft.Extensions.Hosting;

namespace graduationProject.Services
{
    public class OfferService : IOfferService
    {
        private readonly ApplicationDbContext _context;
        private List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long maxAllowedSize = 1048576;
        private readonly string _uploadsPath;
        public OfferService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _uploadsPath = Path.Combine(env.WebRootPath, "cardId");
        }
        public async Task<ResultDto> sendOfferToPost(offerDto Offer)
        {
            var post = await _context.Posts.FindAsync(Offer.postId);
            if (post == null)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = "post not found"
                };
            }
            if (Offer.Image != null)
            {
                var extension = Path.GetExtension(Offer.Image.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ResultDto()
                    {
                        IsSuccess = false,
                        Message = "image not found"
                    };
                }
                if (Offer.Image.Length > maxAllowedSize)
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
                    await Offer.Image.CopyToAsync(fileStream);
                }
                var offer = new offer
                {
                    PostId = Offer.postId,
                    //OfferContent = offerContent,
                    NationalcardInvestor = fileName,
                    Price = Offer.Price,
                    ProfitRate = Offer.ProfitRate,
                    NationalIdInvestor = Offer.NationalId,
                    Description = Offer.Description,
                    //Post.IsHaveOffer = true

                };
                var posttt = await _context.Posts.FindAsync(Offer.postId);

                if (post != null)
                {
                    offer.Post = posttt;
                    posttt.IsHaveOffer = true; 
                }
                _context.Offers.Add(offer);
              //  _context.Offers.Add(offer);
                await _context.SaveChangesAsync();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "offer sent successfully"
                };
                // offer.Image = fileName;
            }
            return new ResultDto()
            {
                IsSuccess = false,
                Message = "offer doesnot sent"
            };




        }
    }
}
