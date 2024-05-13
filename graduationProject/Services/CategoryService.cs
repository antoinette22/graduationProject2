using graduationProject.core.DbContext;
using graduationProject.Dtos;
using graduationProject.DTOs.CategoryDtos;
using graduationProject.Models;
using graduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace graduationProject.Services.Implementations
{
    public class CategoryService: ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _uploadsPath;

        public CategoryService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        }

        public async Task<ReturnCategoryDto> AddCategory(AddCategoryDto dto)
        {
            var returncategory = new ReturnCategoryDto();
            if (_context.Categories.Any(c => c.Name == dto.Name))
            {
                returncategory.Errors = new string[] { "" };
                return returncategory;
            }
            var category = new Category
            {
                Name = dto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            returncategory.isSucces = true;
            returncategory.Name = category.Name;
            returncategory.Id = category.Id;
            return returncategory;
        }
        public async Task<ReturnCategoryDto> EditCategory(CategoryDto dto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (category == null)
            {
                return new ReturnCategoryDto
                {
                    isSucces = false,
                    Errors = new string[] { "Category not found" }
                };
            }
            category.Name = dto.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return new ReturnCategoryDto
            {
                isSucces = true,
                Id = category.Id,
                Name = category.Name
            };
        }
        public async Task<ReturnCategoryDto> GetCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return new ReturnCategoryDto
                {
                    isSucces = false,
                    Errors = new string[] { "Category not found" }
                };
            }
            return new ReturnCategoryDto
            {
                isSucces = true,
                Id = category.Id,
                Name = category.Name
            };
        }
        public async Task<IEnumerable<ReturnCategoryDto>> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var returnCategories = new List<ReturnCategoryDto>();

            foreach (var category in categories)
            {
                var returnCategory = new ReturnCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                };
                returnCategories.Add(returnCategory);
            }
            return returnCategories;
        }
        public async Task<ReturnPostsDto> GetCategoryPosts(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return new ReturnPostsDto
                {
                    isSucces = false,
                    Errors = new string[] { "Category not found" }
                };
            }

            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Attachments)
                .Include(p => p.Category)
                .Where(p => p.Category == category)
                .ToListAsync();

            var returnPosts = posts.Select(post => new ReturnPostDto
            {
                Id = post.Id,
                Content = post.Content,
                UserName = post.User.UserName,
                IsSuccess = true,
                CreatedAt = post.CreatedAt,
                CategoryId = post.Category.Id,
                Reacts = post.Reacts != null ? post.Reacts.Count() : 0,
                Attachments = post.Attachments != null
                    ? post.Attachments.Select(a => Path.Combine(_uploadsPath, a.FilePath)).ToList()
                    : new List<string>()
            }).ToList();

            var returnPostsDto = new ReturnPostsDto
            {
                isSucces = true,
                Posts = returnPosts
            };

            return returnPostsDto;
        }



    }
}
