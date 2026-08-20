using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Services;
using System.Data;


namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICurrentUserService _currentUser;

        public MenuItemController(IMenuItemService menuItemService, ICurrentUserService currentUser)
        {
            _menuItemService = menuItemService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("menuitem", "read")]
        public async Task<IActionResult> GetAll()
        {
            var menuItems = await _menuItemService.GetAllAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<MenuItemResponse>>.Ok((menuItems.Select(mi => MapToResponse(mi)))));
        }

        [HttpGet("{id}")]
        [RequiresPermission("menuitem", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id, _currentUser.TenantId);
            if (menuItem == null)
                return NotFound(ApiResponse<MenuItemResponse>.Fail("Menu item not found."));
            return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem)));
        }

        [HttpGet("course/{course}")]
        [RequiresPermission("menuitem", "read")]
        public async Task<IActionResult> GetByCourse(string course)
        {
            var menuItems = await _menuItemService.GetByCourseAsync(_currentUser.TenantId, course);
            return Ok(ApiResponse<IEnumerable<MenuItemResponse>>.Ok(menuItems.Select(mi => MapToResponse(mi))));
        }

        [HttpGet("status/{status}")]
        [RequiresPermission("menuitem", "read")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var menuItems = await _menuItemService.GetByStatusAsync(_currentUser.TenantId, status);
            return Ok(ApiResponse<IEnumerable<MenuItemResponse>>.Ok(menuItems.Select(mi => MapToResponse(mi))));
        }

        [HttpPost]
        [RequiresPermission("menuitem", "create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest request)
        {
            try
            {
                var menuItem = await _menuItemService.CreateAsync(
                    request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Menu item created."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequest request)
        {
            try
            {
                var menuItem = await _menuItemService.UpdateAsync(
                    id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Meny item updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail("Menu item not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("menuitem", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _menuItemService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Menu item deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Menu item not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/publish")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> Publish(Guid id)
        {
            try
            {
                var menuItem = await _menuItemService.PublishAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Meny item published."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Meny itme not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/recipes")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> AddRecipe(Guid id, [FromBody] AddMenuItemRecipeRequest request)
        {
            try
            {
                var menuItem = await _menuItemService.AddRecipeAsync(id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Recipe linked to menu item."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/recipes/{recipeId}")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> RemoveRecipes(Guid id, Guid recipeId)
        {
            try
            {
                var menuItem = await _menuItemService.RemoveRecipeAsync(
                    id, recipeId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(
                    MapToResponse(menuItem), "Recipe removed from menu item."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/resolve-allergens")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> ResolveAllergens(Guid id)
        {
            try
            {
                var menyItem = await _menuItemService.ResolveAllergensAsync(
                    id, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(
                    MapToResponse(menyItem), "Allergens resolved."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail("Menu item not found."));
            }
        }

        [HttpPost("{id}/allergens")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> AddManualAlleren(
            Guid id, [FromBody] AddMenuItemAllergenRequest request)
        {
            try
            {
                var menuItem = await _menuItemService.AddManualAllergenAsync(id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Allergen manually added."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/allergens/{allergenId}")]
        [RequiresPermission("menuitem", "update")]
        public async Task<IActionResult> RemoveManualAllergen(Guid id, Guid allergenId)
        {
            try
            {
                var menuItem = await _menuItemService.RemoveManualAllergenAsync(
                    id, allergenId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<MenuItemResponse>.Ok(MapToResponse(menuItem), "Allergen removed."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<MenuItemResponse>.Fail(ex.Message));
            }
        }


        private static MenuItemResponse MapToResponse(MenuItem mi) => new()
        {
            MenuItemId = mi.MenuItemId,
            TenantId = mi.TenantId,
            Name = mi.Name,
            Description = mi.Description,
            Course = mi.Course,
            Status = mi.Status,
            IsActive = mi.IsActive,
            CreatedBy = mi.CreatedBy,
            CreatedByName = mi.CreatedByUser != null
                ? $"{mi.CreatedByUser.FirstName} {mi.CreatedByUser.LastName}" : null,
            CreatedAt = mi.CreatedAt,
            UpdatedAt = mi.UpdatedAt,
            Recipes = mi.MenuItemRecipes.Select(mir => new MenuItemRecipeResponse
            {
                MenuItemRecipeId = mir.MenuItemRecipeId,
                RecipeId = mir.RecipeId,
                RecipeTitle = mir.Recipe.Title,
                RecipeStatus = mir.Recipe.Status,
                DisplayOrder = mir.DisplayOrder,
                Note = mir.Note
            }).OrderBy(r => r.DisplayOrder).ToList(),
            Allergens = mi.MenuItemAllergens.Select(mia => new MenuItemAllergenResponse
            {
                MenuItemAllergenId = mia.MenuItemAllergenId,
                AllergenId = mia.AllergenId,
                AllergenName = mia.AllergenTag.Name,
                IsMajor = mia.AllergenTag.IsMajor,
                SourceName = mia.SourceName,
                SourceComponent = mia.SourceComponent,
                IsDirect = mia.IsDirect,
                IsManual = mia.IsManual
            }).ToList()
        };
    }
}
