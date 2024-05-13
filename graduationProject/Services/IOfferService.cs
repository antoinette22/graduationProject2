using graduationProject.Dtos;
using graduationProject.DTOs;
using graduationProject.DTOs.OfferDtos;

namespace graduationProject.Services
{
    public interface IOfferService
    {
        Task<ResultDto> sendOfferToPost(offerDto Offer);
    }
}
