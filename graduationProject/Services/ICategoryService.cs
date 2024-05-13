using graduationProject.Dtos;
using graduationProject.DTOs.CategoryDtos;
using graduationProject.Models;
using Microsoft.EntityFrameworkCore;

namespace graduationProject.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ReturnCategoryDto> AddCategory(AddCategoryDto dto);
        Task<ReturnCategoryDto> EditCategory(CategoryDto dto);
        Task<ReturnCategoryDto> GetCategory(int id);
        Task<IEnumerable<ReturnCategoryDto>> GetAllCategories();
        Task<ReturnPostsDto> GetCategoryPosts(int id);
    }
}
