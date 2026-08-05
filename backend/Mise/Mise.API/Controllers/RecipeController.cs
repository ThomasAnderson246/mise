using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<RecipeController> _logger;
        private readonly ISubRecipeService _subRecipeService;

        public RecipeController(
            IRecipeService recipeService, ISubRecipeService subRecipeService, ICurrentUserService currentUser, ILogger<RecipeController> logger)
        {
            _recipeService = recipeService;
            _subRecipeService = subRecipeService;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _recipeService.GetAllAsync(_currentUser.TenantId);

            var response = recipes.Select(r => new RecipeListResponse
            {
                RecipeId = r.RecipeId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                ScalingMode = r.ScalingMode,
                TenantId = r.TenantId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                RecipeCategories = r.RecipeCategories.Select(rc => new RecipeCategoryResponse
                {
                    CategoryId = rc.Category.CategoryId,
                    Name = rc.Category.Name
                }).ToList()
            });

            return Ok(ApiResponse<IEnumerable<RecipeListResponse>>.Ok(response));
        }

        [HttpGet("{id}")]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var recipe = await _recipeService.GetByIdAsync(id, _currentUser.TenantId);

            if (recipe == null)
                return NotFound(ApiResponse<RecipeResponse>.Fail("Recipe not found."));

            var response = new RecipeDetailResponse
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Status = recipe.Status,
                ScalingMode = recipe.ScalingMode,
                TenantId = recipe.TenantId,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                RecipeCategories = recipe.RecipeCategories.Select(rc => new RecipeCategoryResponse
                {
                    CategoryId = rc.CategoryId,
                    Name = rc.Category.Name
                }).ToList(),
                CurrentVersion = recipe.CurrentVersion == null ? null : new RecipeVersionResponse
                {
                    VersionId = recipe.CurrentVersion.VersionId,
                    VersionNumber = recipe.CurrentVersion.VersionNumber,
                    IsDraft = recipe.CurrentVersion.IsDraft,
                    IsPublished = recipe.CurrentVersion.IsPublished,
                    RecipeIngredientGroups = recipe.CurrentVersion.IngredientGroups
                        .OrderBy(g => g.DisplayOrder)
                        .Select(g => new RecipeIngredientGroupResponse
                        {
                            GroupId = g.GroupId,
                            Name = g.Name,
                            DisplayOrder = g.DisplayOrder,
                            Ingredients = recipe.CurrentVersion.Ingredients
                                .Where(ri => ri.GroupId == g.GroupId)
                                .OrderBy(ri => ri.DisplayOrder)
                                .Select(ri => new RecipeIngredientResponse
                                {
                                    RecipeIngredientId = ri.RecipeIngredientId,
                                    IngredientId = ri.IngredientId,
                                    IngredientName = ri.Ingredient?.Name ?? string.Empty,
                                    Quantity = ri.Quantity,
                                    UnitName = ri.UnitType?.Name,
                                    UnitTypeId = ri.UnitTypeId,
                                    DisplayOrder = ri.DisplayOrder,
                                    GroupId = ri.GroupId
                                }).ToList()

                        }).ToList(),
                    Ingredients = recipe.CurrentVersion.Ingredients
                        .Where(ri => ri.GroupId == null)
                        .OrderBy(ri => ri.DisplayOrder)
                        .Select(ri => new RecipeIngredientResponse
                        {
                            RecipeIngredientId = ri.RecipeIngredientId,
                            IngredientName = ri.Ingredient.Name ?? string.Empty,
                            Quantity = ri.Quantity,
                            UnitName = ri.UnitType?.Name,
                            DisplayOrder = ri.DisplayOrder,
                            GroupId = ri.GroupId
                        }).ToList(),
                    Steps = recipe.CurrentVersion.Steps
                        .OrderBy(s => s.StepNumber)
                        .Select(s => new RecipeStepResponse
                        {
                            StepId = s.StepId,
                            StepNumber = s.StepNumber,
                            Instruction = s.Instruction,
                            HasTimer = s.HasTimer,
                            TimerDuration = s.TimerDuration,
                            IsAsync = s.IsAsync,
                            AsyncGroupId = s.AsyncGroupId
                        }).ToList()
                }
            };

            return Ok(ApiResponse<RecipeDetailResponse>.Ok(response));
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeRequest request)
        {
            _logger.LogInformation("Update recipe {Id} with categories: {Categories}",
            id, string.Join(", ", request.CategoryIds ?? new List<Guid>()));
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

        [HttpGet("{id}/subrecipes")]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetSubRecipes(Guid id)
        {
            var subRecipes = await _subRecipeService.GetByParentAsync(id, _currentUser.TenantId);

            var response = subRecipes.Select(sr => new SubRecipeResponse
            {
                ParentRecipeId = sr.ParentRecipeId,
                SubRecipeId = sr.SubRecipeId,
                SubRecipeTitle = sr.ChildRecipe.Title,
                SubRecipeStatus = sr.ChildRecipe.Status
            });

            return Ok(ApiResponse<IEnumerable<SubRecipeResponse>>.Ok(response));
        }

        [HttpPost("{id}/subrecipes")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> AddSubRecipe(Guid id, [FromBody] AddSubRecipeRequest request)
        {
            try
            {
                await _subRecipeService.AddAsync(
                    id, request.SubRecipeId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<string>.Ok("Sub-recipe added.", "Sub-recipe added successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/subrecipes/{subRecipeId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> RemoveSubRecipe(Guid id, Guid subRecipeId)
        {
            try
            {
                await _subRecipeService.RemoveAsync(
                    id, subRecipeId, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Sub-recipe removed.", "Sub-recipe removed successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/ingredients")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> AddIngredient(Guid id, [FromBody] AddRecipeIngredientRequest request)
        {
            try
            {
                await _recipeService.AddIngredientAsync(id, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Ingredient added.", "Ingredient added to recipe."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/ingredients/{recipeIngredientId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> UpdateIngredient(Guid id, Guid recipeIngredientId, [FromBody] UpdateRecipeIngredientRequest request)
        {
            try
            {
                await _recipeService.UpdateIngredientAsync(id, recipeIngredientId, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Ingredient updated.", "Ingredient updated."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/ingredients/{recipeIngredientId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> RemoveIngredient(Guid id, Guid recipeIngredientId)
        {
            try
            {
                await _recipeService.RemoveIngredientAsync(id, recipeIngredientId, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Ingredient removed.", "Ingredient removed."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/steps")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> AddStep(Guid id, [FromBody] AddRecipeStepRequest request)
        {
            try
            {
                await _recipeService.AddStepAsync(id, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Step added.", "Step added to recipe."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/steps/{stepId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> UpdateStep(Guid id, Guid stepId, [FromBody] UpdateRecipeStepRequest request)
        {
            try
            {
                await _recipeService.UpdateStepAsync(id, stepId, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Step updated.", "Step updated."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/steps{stepId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> RemoveStep(Guid id, Guid stepId)
        {
            try
            {
                await _recipeService.RemoveStepAsync(id, stepId, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("step removed.", "Step removed from recipe."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/groups")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> AddIngredientGroup(Guid id, [FromBody] AddRecipeIngredientGroupRequest request)
        {
            try
            {
                await _recipeService.AddIngredientGroupAsync(id, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Group added.", "Ingredient group added to recipe."));

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/groups/{groupId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> RemoveIngredientGroup(Guid id, Guid groupId)
        {
            try
            {
                await _recipeService.RemoveIngredientGroupAsync(id, groupId, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Group removed.", "Ingredient group removed."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet("{id}/draft")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> GetDraft(Guid id)
        {
            var draft = await _recipeService.GetDraftVersionAsync(id, _currentUser.TenantId);
            if (draft == null)
                return NotFound(ApiResponse<RecipeVersionSummaryResponse>.Fail("No draft found."));

            return Ok(ApiResponse<RecipeDetailResponse>.Ok(MapVersionToResponse(draft)));
        }

        [HttpPost("{id}/draft")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> CreateDraft(Guid id)
        {
            try
            {
                var draft = await _recipeService.CreateDraftFromCurrentAsync(
                    id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<RecipeVersionSummaryResponse>.Ok(
                    MapVersionSummaryToResponse(draft, null), "Draft Created."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/draft/{versionId}")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> SaveDraft(Guid id, Guid versionId, [FromBody] SaveDraftRequest request)
        {
            _logger.LogInformation("SaveDraft called - RecipeId: {Id}, VersionId: {VersionId}, Ingredients: {IngCount}, Steps: {StepCount}",
                id, versionId, request.Ingredients?.Count ?? 0, request.Steps?.Count ?? 0);
            try
            {
                await _recipeService.SaveDraftAsync(id, versionId, request, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Draft saved.", "Draft saved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveDraft error: {Message}", ex.Message);
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/versions/{versionId}/restore")]
        [RequiresPermission("recipe", "publish")]
        public async Task<IActionResult> RestoreVersion(Guid id, Guid versionId)
        {
            try
            {
                await _recipeService.RestoreVersionAsync(id, versionId, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Version restored.", "Version restored successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/draft")]
        [RequiresPermission("recipe", "update")]
        public async Task<IActionResult> DiscardDraft(Guid id)
        {
            try
            {
                await _recipeService.DiscardDraftAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Draft discarded.", "Draft discarded successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet("{id}/versions")]
        [RequiresPermission("recipe", "read")]
        public async Task<IActionResult> GetVersionHistory(Guid id)
        {
            var recipe = await _recipeService.GetByIdAsync(id, _currentUser.TenantId);
            if (recipe == null)
                return NotFound(ApiResponse<string>.Fail("Recipe not found."));

            var versions = await _recipeService.GetVersionHistoryAsync(id, _currentUser.TenantId);

            var response = versions.Select(v => MapVersionSummaryToResponse(v, recipe.CurrentVersionId));
            return Ok(ApiResponse<IEnumerable<RecipeVersionSummaryResponse>>.Ok(response));
        }

        private static RecipeVersionSummaryResponse MapVersionSummaryToResponse(
            RecipeVersion v, Guid? currentVersionId) => new()
            {
                VersionId = v.VersionId,
                VersionNumber = v.VersionNumber,
                IsDraft = v.IsDraft,
                IsPublished = v.IsPublished,
                IsCurrent = currentVersionId.HasValue && v.VersionId == currentVersionId.Value,
                CreatedAt = v.CreatedAt,
                PublishedAt = v.PublishedAt,
                PublishedByName = v.PublishedByUser != null
                    ? $"{v.PublishedByUser.FirstName} {v.PublishedByUser.LastName}"
                    : null
            };
        
        private static RecipeDetailResponse MapVersionToResponse(RecipeVersion v) => new()
        {
            RecipeId = v.RecipeId,
            Title = v.Recipe?.Title ?? string.Empty,
            Status = v.IsDraft ? "draft" : "published",
            CurrentVersion = new RecipeVersionResponse
            {
                VersionId = v.VersionId,
                VersionNumber = v.VersionNumber,
                IsDraft = v.IsDraft,
                IsPublished = v.IsPublished,
                Ingredients = v.Ingredients
                    .Where(ri => ri.GroupId == null)
                    .OrderBy(ri => ri.DisplayOrder)
                    .Select(ri => new RecipeIngredientResponse
                    {
                        RecipeIngredientId = ri.RecipeIngredientId,
                        IngredientId = ri.IngredientId,
                        IngredientName = ri.Ingredient?.Name ?? string.Empty,
                        Quantity = ri.Quantity,
                        UnitName = ri.UnitType?.Name,
                        UnitTypeId = ri.UnitTypeId,
                        DisplayOrder = ri.DisplayOrder,
                        GroupId = ri.GroupId
                    }).ToList(),
                Steps = v.Steps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new RecipeStepResponse
                    {
                        StepId = s.StepId,
                        StepNumber = s.StepNumber,
                        Instruction = s.Instruction,
                        HasTimer = s.HasTimer,
                        TimerDuration = s.TimerDuration,
                        IsAsync = s.IsAsync,
                        AsyncGroupId = s.AsyncGroupId
                    }).ToList(),
                RecipeIngredientGroups = v.IngredientGroups
                    .OrderBy(g => g.DisplayOrder)
                    .Select(g => new RecipeIngredientGroupResponse
                    {
                        GroupId = g.GroupId,
                        Name = g.Name,
                        DisplayOrder = g.DisplayOrder,
                        Ingredients = v.Ingredients
                            .Where(ri => ri.GroupId == g.GroupId)
                            .OrderBy(ri => ri.DisplayOrder)
                            .Select(ri => new RecipeIngredientResponse
                            {
                                RecipeIngredientId = ri.RecipeIngredientId,
                                IngredientId = ri.IngredientId,
                                IngredientName = ri.Ingredient?.Name ?? string.Empty,
                                Quantity = ri.Quantity,
                                UnitName = ri.UnitType?.Name,
                                UnitTypeId = ri.UnitTypeId,
                                DisplayOrder = ri.DisplayOrder,
                                GroupId = ri.GroupId
                            }).ToList()
                    }).ToList()
            }
        };
    }
}
