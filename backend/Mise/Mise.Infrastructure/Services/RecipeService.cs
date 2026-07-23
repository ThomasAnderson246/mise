using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;
using System.Runtime.InteropServices;

namespace Mise.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly MiseDbContext _context;
        private readonly IAuditLogServices _auditLogServices;
        private readonly INotificationService _notificationService;

        public RecipeService(IRecipeRepository reciperRepository, MiseDbContext context, IAuditLogServices auditLogServices, INotificationService notificationService)
        {
            _recipeRepository = reciperRepository;
            _context = context;
            _auditLogServices = auditLogServices;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync(Guid tenantId)
        {
            return await _recipeRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<Recipe?> GetByIdAsync(Guid recipeId, Guid tenantId)
        {
            return await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId);
        }

        public async Task<Recipe> CreateAsync(
            CreateRecipeRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            var recipe = new Recipe
            {
                RecipeId = Guid.NewGuid(),
                TenantId = tenantId,
                Title = request.Title,
                Description = request.Description,
                Status = "draft",
                ScalingMode = request.ScalingMode,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _recipeRepository.AddAsync(recipe);

            // Create initial draft version of the recipe
            var version = new RecipeVersion
            {
                VersionId = Guid.NewGuid(),
                RecipeId = recipe.RecipeId,
                VersionNumber = 1,
                IsDraft = true,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.RecipeVersions.AddAsync(version);

            // set the current version
            recipe.CurrentVersionId = version.VersionId;
            await _recipeRepository.UpdateAsync(recipe);

            // add categories if provided
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var recipeCategories = request.CategoryIds.Select(cId => new RecipeCategory
                {
                    RecipeId = recipe.RecipeId,
                    CategoryId = cId
                }).ToList();

                await _context.RecipeCategories.AddRangeAsync(recipeCategories);
                await _context.SaveChangesAsync();
                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create",
                    "recipe",
                    recipe.RecipeId,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        recipe.Title,
                        recipe.Description,
                        recipe.ScalingMode,
                        recipe.Status
                    }));
            }

            return recipe;
        }

        public async Task<Recipe> UpdateAsync(
            Guid recipeId,
            UpdateRecipeRequest request,
            Guid tenantId, Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var previousState = JsonSerializer.Serialize(new
            {
                recipe.Title,
                recipe.Description,
                recipe.ScalingMode,
                recipe.Status
            });

            if (request.Title != null) recipe.Title = request.Title;
            if (request.Description != null) recipe.Description = request.Description;
            if (request.ScalingMode != null) recipe.ScalingMode = request.ScalingMode;
            recipe.UpdatedAt = DateTime.UtcNow;

            await _recipeRepository.UpdateAsync(recipe);

            //update the categories if they're provided.

            if (request.CategoryIds != null)
            {
                var exisitng = _context.RecipeCategories
                    .Where(rc => rc.RecipeId == recipeId);
                _context.RecipeCategories.RemoveRange(exisitng);

                

                var newCategories = request.CategoryIds.Select(cId => new RecipeCategory
                {
                    RecipeId = recipeId,
                    CategoryId = cId,
                }).ToList();

                await _context.RecipeCategories.AddRangeAsync(newCategories);
                await _context.SaveChangesAsync();

                var newState = JsonSerializer.Serialize(new
                {
                    recipe.Title,
                    recipe.Description,
                    recipe.ScalingMode,
                    recipe.Status
                });

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "update",
                    "recipe",
                    recipe.RecipeId,
                    previousState,
                    newState);
            }

            return recipe;
        }

        public async Task DeleteAsync(Guid recipeId, Guid tenantId, Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var previousState = JsonSerializer.Serialize(new
            {
                recipe.Title,
                recipe.Description,
                recipe.ScalingMode,
                recipe.Status
            });

            await _recipeRepository.DeleteAsync(recipeId);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "delete",
                "recipe",
                recipeId,
                previousState,
                null);
        }

        public async Task<Recipe> PublishAsync(
            Guid recipeId,
            Guid tenantId,
            Guid publishedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            if (recipe.CurrentVersionId == null)
                throw new InvalidOperationException("Recipe has no version to publish.");

            var version = await _context.RecipeVersions
                .FirstOrDefaultAsync(rv => rv.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException("Recipe version not found.");

            version.IsDraft = false;
            version.IsPublished = true;
            version.PublishedBy = publishedBy;
            version.PublishedAt = DateTime.UtcNow;

            var wasAlreadyPublished = recipe.Status == "published";

            recipe.Status = "published";
            recipe.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            if (wasAlreadyPublished)
            {
                await _notificationService.NotifyRecipeUpdatedAsync(
                    recipeId, recipe.Title, tenantId, publishedBy);
            }
            else
            {
                await _notificationService.NotifyRecipePublishedAsync(
                    recipeId,
                    recipe.Title,
                    tenantId,
                    publishedBy);
            }

            

            await _auditLogServices.LogAsync(
                tenantId,
                publishedBy,
                "publish",
                "recipe",
                recipeId,
                null,
                JsonSerializer.Serialize(new
                {
                    recipe.Title,
                    recipe.Description
                }));

            return recipe;
        }

        public async Task<Recipe> AddIngredientAsync(
            Guid recipeId,
            AddRecipeIngredientRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            if (recipe.CurrentVersionId == null)
                throw new InvalidOperationException("Recipe has no current version.");

            var ingredient = new RecipeIngredient
            {
                RecipeIngredientId = Guid.NewGuid(),
                VersionId = recipe.CurrentVersionId.Value,
                IngredientId = request.IngredientId,
                Quantity = request.Quantity,
                UnitTypeId = request.UnitTypeId,
                DisplayOrder = request.DisplayOrder,
                GroupId = request.GroupId,
                IsNonConvertible = request.IsNonConvertible,
                IsRatioAnchor = request.IsRatioAnchor,
            };

            await _context.RecipeIngredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "add_ingredient",
                "recipe",
                recipeId,
                null,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    request.IngredientId,
                    request.Quantity
                }));

            return recipe;
        }

        public async Task<Recipe> UpdateIngredientAsync(
            Guid recipeId,
            Guid recipeIngredientId,
            UpdateRecipeIngredientRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var ingredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.RecipeIngredientId == recipeIngredientId
                    && ri.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException($"Ingredient {recipeIngredientId} not found.");

            var previousState = System.Text.Json.JsonSerializer.Serialize(new
            {
                ingredient.Quantity,
                ingredient.UnitTypeId,
                ingredient.DisplayOrder
            });

            if (request.Quantity != null) ingredient.Quantity = request.Quantity.Value;
            if (request.UnitTypeId != null) ingredient.UnitTypeId = request.UnitTypeId;
            if (request.DisplayOrder != null) ingredient.DisplayOrder = request.DisplayOrder.Value;
            if (request.GroupId != null) ingredient.GroupId = request.GroupId;

            _context.RecipeIngredients.Update(ingredient);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "update_ingredient",
                "recipe",
                recipeId,
                previousState,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    ingredient.Quantity,
                    ingredient.UnitTypeId,
                    ingredient.DisplayOrder
                }));

            return recipe;
        }

        public async Task<Recipe> RemoveIngredientAsync(
            Guid recipeId,
            Guid recipeIngredientId,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var ingredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.RecipeIngredientId == recipeIngredientId
                    && ri.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException($"Ingredient {recipeIngredientId} not found.");

            _context.RecipeIngredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "remove_ingredient",
                "recipe",
                recipeId,
                System.Text.Json.JsonSerializer.Serialize(new { recipeIngredientId }),
                null);

            return recipe;
        }

        public async Task<Recipe> AddStepAsync(
            Guid recipeId,
            AddRecipeStepRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            if (recipe.CurrentVersionId == null)
                throw new InvalidOperationException("Recipe has no current version.");

            var step = new RecipeStep
            {
                StepId = Guid.NewGuid(),
                VersionId = recipe.CurrentVersionId.Value,
                StepNumber = request.StepNumber,
                Instruction = request.Instruction,
                HasTimer = request.HasTimer,
                TimerDuration = request.TimerDuration,
                IsAsync = request.IsAsync,
                AsyncGroupId = request.AsyncGroupId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.RecipeSteps.AddAsync(step);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "add_step",
                "recipe",
                recipeId,
                null,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    step.StepNumber,
                    step.Instruction
                }));

            return recipe;
        }

        public async Task<Recipe> UpdateStepAsync(
            Guid recipeId,
            Guid stepId,
            UpdateRecipeStepRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var step = await _context.RecipeSteps
                .FirstOrDefaultAsync(s => s.StepId == stepId
                    && s.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException($"Step {stepId} not found.");

            var previousState = JsonSerializer.Serialize(new
            {
                step.StepNumber,
                step.Instruction,
                step.HasTimer,
                step.TimerDuration
            });

            if (request.Instruction != null) step.Instruction = request.Instruction;
            if (request.StepNumber != null) step.StepNumber = request.StepNumber.Value;
            if (request.HasTimer != null) step.HasTimer = request.HasTimer.Value;
            if (request.TimerDuration != null) step.TimerDuration = request.TimerDuration.Value;
            if (request.IsAsync != null) step.IsAsync = request.IsAsync.Value;
            if (request.AsyncGroupId != null) step.AsyncGroupId = request.AsyncGroupId.Value;

            _context.RecipeSteps.Update(step);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "update_step",
                "recipe",
                recipeId,
                previousState,
                JsonSerializer.Serialize(new
                {
                    step.StepNumber,
                    step.Instruction,
                    step.HasTimer,
                    step.TimerDuration
                }));

            return recipe;
        }

        public async Task<Recipe> RemoveStepAsync(
            Guid recipeId,
            Guid stepId,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var step = await _context.RecipeSteps
                .FirstOrDefaultAsync(s => s.StepId == stepId
                    && s.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException($"step {stepId} not found.");

            _context.RecipeSteps.Remove(step);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "remove_step",
                "recipe",
                recipeId,
                JsonSerializer.Serialize(new { stepId }),
                null);

            return recipe;
        }

        public async Task<Recipe> AddIngredientGroupAsync(
            Guid recipeId,
            AddRecipeIngredientGroupRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            if (recipe.CurrentVersionId == null)
                throw new InvalidOperationException("recipe has no current version.");

            var group = new RecipeIngredientGroup
            {
                GroupId = Guid.NewGuid(),
                VersionId = recipe.CurrentVersionId.Value,
                Name = request.Name,
                DisplayOrder = request.DisplayOrder
            };

            await _context.RecipeIngredientGroups.AddAsync(group);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "add_ingredient_group",
                "recipe",
                recipeId,
                null,
                JsonSerializer.Serialize(new { group.Name }));

            return recipe;
        }

        public async Task<Recipe> RemoveIngredientGroupAsync(
            Guid recipeId,
            Guid groupId,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var group = await _context.RecipeIngredientGroups
                .FirstOrDefaultAsync(g => g.GroupId == groupId
                    && g.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException($"Grup {groupId} not found.");

            _context.RecipeIngredientGroups.Remove(group);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "remove_ingredient_group",
                "recipe",
                recipeId,
                JsonSerializer.Serialize(new { group.Name }),
                null);

            return recipe;
        }
    
    
    
    }

    
}
