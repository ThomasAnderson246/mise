using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class IngredientController : ControllerBase
    {

        private readonly IIngredientService _ingredientService;
        private readonly ICurrentUserService _currentUser;

        public IngredientController(IIngredientService ingredientService, ICurrentUserService currentUser)
        {
            _ingredientService = ingredientService;
            _currentUser = currentUser;
        }

        [HttpGet("id")]
        [RequiresPermission("ingredient", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id, _currentUser.TenantId);

            if (ingredient == null)
                return NotFound(ApiResponse<IngredientResponse>.Fail("Ingredient not found."));

            return Ok(ApiResponse<IngredientResponse>.Ok(MapToResponse(ingredient)));
        }

        [HttpGet("search")]
        [RequiresPermission("ingredient", "read")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var ingredients = await _ingredientService.SearchAsync(_currentUser.TenantId, term);
            return Ok(ApiResponse<IEnumerable<IngredientResponse>>.Ok(ingredients.Select(MapToResponse)));
        }

        [HttpPost]
        [RequiresPermission("ingredient", "create")]
        public async Task<IActionResult> Create([FromBody] CreateIngredientRequest request)
        {
            var ingredient = await _ingredientService.CreateAsync(
                request, _currentUser.TenantId, _currentUser.UserId);

            return Ok(ApiResponse<IngredientResponse>.Ok(MapToResponse(ingredient), "Ingredient created."));
        }

        [HttpPut("{id}")]
        [RequiresPermission("ingredient", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngredientRequest request)
        {
            try
            {
                var ingredient = await _ingredientService.UpdateAsync(
                    id, request, _currentUser.TenantId);

                return Ok(ApiResponse<IngredientResponse>.Ok(MapToResponse(ingredient), "Ingredient updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<IngredientResponse>.Fail("Ingredient not found"));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("ingredient", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _ingredientService.DeleteAsync(id, _currentUser.TenantId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Ingredient deleted."));
            }
            catch
            {
                return NotFound(ApiResponse<string>.Fail("Ingredient not found."));
            }
        }

        private static IngredientResponse MapToResponse(Domain.Entities.Ingredient i) => new()
        {
            IngredientId = i.IngredientId,
            TenantId = i.TenantId,
            Name = i.Name,
            Category = i.Category,
            DefaultUnitTypeId = i.DefaultUnitTypeId,
            DefaultUnittypeName = i.DefaultUnitType?.Name,
            IsNonConvertible = i.IsNonConvertible,
            Allergens = i.IngredientAllergens.Select(ia => new AllergenTagResponse
            {
                AllergenId = ia.AllergenTag.AllergenId,
                Name = ia.AllergenTag.Name,
                Description = ia.AllergenTag.Description,
                IsMajor = ia.AllergenTag.IsMajor
            }).ToList(),
            CreatedAt= i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
        };
    }
}
