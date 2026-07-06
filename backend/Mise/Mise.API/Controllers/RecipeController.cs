using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(
            IRecipeService recipeService, ICurrentUserService currentUser, ILogger<RecipeController> logger)
        {
            _recipeService = recipeService;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _recipeService.GetAllAsync(_currentUser.TenantId);

            var response = recipes.Select(r => new RecipeResponse
            {
                RecipeId = r.RecipeId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                ScalingMode = r.ScalingMode,
                TenantId = r.TenantId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
            });

            return Ok(ApiResponse<IEnumerable<RecipeResponse>>.Ok(response));
        }

        [HttpGet("{id}")]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var recipe = await _recipeService.GetByIdAsync(id, _currentUser.TenantId);

            if (recipe == null)
                return NotFound(ApiResponse<RecipeResponse>.Fail("Recipe not found."));

            var response = new RecipeResponse
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Status = recipe.Status,
                ScalingMode = recipe.ScalingMode,
                TenantId = recipe.TenantId,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt
            };

            return Ok(ApiResponse<RecipeResponse>.Ok(response));
        }

        [HttpPost]
        [RequiresPermission("recipe", "create")]
        public async Task<IActionResult> Create([FromBody] CreateRecipeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<RecipeResponse>.Fail("Invalid request."));

            var recipe = await _recipeService.CreateAsync(
                request,
                _currentUser.TenantId,
                _currentUser.UserId);

            var response = new RecipeResponse
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Status = recipe.Status,
                ScalingMode = recipe.ScalingMode,
                TenantId = recipe.TenantId,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = recipe.RecipeId },
                ApiResponse<RecipeResponse>.Ok(response, "Recipe created successfully."));
        }

        [HttpPut("{id}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody ] UpdateRecipeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<RecipeResponse>.Fail("Invalid request."));

            var recipe = await _recipeService.UpdateAsync(id, request, _currentUser.TenantId, _currentUser.UserId);

            var response = new RecipeResponse
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Status = recipe.Status,
                ScalingMode = recipe.ScalingMode,
                TenantId = recipe.TenantId,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt
            };

            return Ok(ApiResponse<RecipeResponse>.Ok(response, "Recipe updated successfully."));
        }

        [HttpDelete("{id}")]
        [RequiresPermission("recipe", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _recipeService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
            return Ok(ApiResponse<string>.Ok("Deleted.", "Recipe deleted successfully."));
        }

        [HttpPost("{id}/publish")]
        [RequiresPermission("recipe", "publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            var recipe = await _recipeService.PublishAsync(
                id,
                _currentUser.TenantId,
                _currentUser.UserId);

            var response = new RecipeResponse
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Status = recipe.Status,
                ScalingMode = recipe.ScalingMode,
                TenantId = recipe.TenantId,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt

            };

            return Ok(ApiResponse<RecipeResponse>.Ok(response, "Recipe published successfully."));
        }
    }
}
