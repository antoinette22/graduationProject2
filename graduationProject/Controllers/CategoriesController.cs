using graduationProject.Dtos;
using graduationProject.DTOs.CategoryDtos;
using graduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace graduationProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result =await _categoryService.GetAllCategories();
            return Ok(result);
        }
        [HttpGet("get-category-posts")]
        public async Task<IActionResult> GetPosts(int id)
        {
            var result = await _categoryService.GetCategoryPosts(id);
            if (!result.isSucces)
            {
                var errors = new { result.Errors};
                return BadRequest(errors);
            }
            return Ok(result.Posts);
        }
        [HttpGet("get-category")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result =await _categoryService.GetCategory(id);
            if (!result.isSucces)
            {
                var errors = new { result.Errors};
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory(AddCategoryDto dto)
        {
            var result =await _categoryService.AddCategory(dto);
            if (!result.isSucces)
            {
                var errors = new { result.Errors};
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("edit-category")]
        public async Task<IActionResult> EditCategory(CategoryDto dto)
        {
            var result = await _categoryService.EditCategory(dto);
            if (!result.isSucces)
            {
                var errors = new { result.Errors};
                return BadRequest(errors);
            }
            return Ok(result);
        }
    }
}
