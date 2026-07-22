using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        private readonly ICurrentUserService _currentUserService;

        public CategoryController(ICategoryService categoryService, ICurrentUserService currentUserService)
        {
            _categoryService = categoryService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [RequiresPermission("category", "read")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetallAsync(_currentUserService.TenantId);
            return Ok(ApiResponse<IEnumerable<CategoryResponse>>.Ok(categories.Select(c => MapToResponse(c))));
        }

        [HttpGet("{id}")]
        [RequiresPermission("category", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetbyIdAsync(id, _currentUserService.TenantId);
            if (category == null)
                return NotFound(ApiResponse<CategoryResponse>.Fail("Category not found."));

            return Ok(ApiResponse<CategoryResponse>.Ok(MapToResponse(category)));
        }

        [HttpPost]
        [RequiresPermission("category", "create")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.CreateAsync(
                    request,
                    _currentUserService.TenantId,
                    _currentUserService.UserId);

                return Ok(ApiResponse<CategoryResponse>.Ok(MapToResponse(category), "Category created."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CategoryResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [RequiresPermission("category", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.UpdateAsync(
                    id,
                    request,
                    _currentUserService.TenantId,
                    _currentUserService.UserId);

                return Ok(ApiResponse<CategoryResponse>.Ok(MapToResponse(category), "Category updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<CategoryResponse>.Fail("Category not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CategoryResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("category", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _categoryService.DeleteAsync(
                    id,
                    _currentUserService.TenantId,
                    _currentUserService.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Category deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("category not found."));
            }
        }

        private static CategoryResponse MapToResponse(Category c) => new()
        {
            CategoryId = c.CategoryId,
            TenantId = c.TenantId,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
        };
    }
}
