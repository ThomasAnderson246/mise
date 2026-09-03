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
using Microsoft.Extensions.Logging;

namespace Mise.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly MiseDbContext _context;
        private readonly IAuditLogServices _auditLogServices;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(IRecipeRepository reciperRepository, MiseDbContext context, IAuditLogServices auditLogServices, INotificationService notificationService, ILogger<RecipeService> logger)
        {
            _recipeRepository = reciperRepository;
            _context = context;
            _auditLogServices = auditLogServices;
            _notificationService = notificationService;
            _logger = logger;
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
                IsPortion = request.IsPortion,
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

            _logger.LogInformation("UpdateAsync - Title: {Title}, Description: {Description}, ScalingMode: {ScalingMode}, CategoryIds: {CategoryIds}",
                request.Title ?? "null",
                request.Description ?? "null",
                request.ScalingMode ?? "null",
                request.CategoryIds == null ? "null" : string.Join(", ", request.CategoryIds));

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
                var existing = await _context.RecipeCategories
                    .Where(rc => rc.RecipeId == recipeId)
                    .ToListAsync();
                _context.RecipeCategories.RemoveRange(existing);

                var newCategories = request.CategoryIds.Select(cId => new RecipeCategory
                {
                    RecipeId = recipeId,
                    CategoryId = cId,
                }).ToList();
                foreach (var cat in newCategories)
                {
                    _logger.LogInformation("Inserting recipe_category: RecipeId={RecipeId}, CategoryId={CategoryId}",
                        cat.RecipeId, cat.CategoryId);
                }
                await _context.RecipeCategories.AddRangeAsync(newCategories);
                await _context.SaveChangesAsync();
            }

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

            // find draft version if it exists, otherwise use the current version
            var draft = await _context.RecipeVersions
                .FirstOrDefaultAsync(rv => rv.RecipeId == recipeId && rv.IsDraft);

            var versionToPublish = draft ?? await _context.RecipeVersions
                .FirstOrDefaultAsync(rv => rv.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException("Recipe version not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                versionToPublish.IsDraft = false;
                versionToPublish.IsPublished = true;
                versionToPublish.PublishedBy = publishedBy;
                versionToPublish.PublishedAt = DateTime.UtcNow;

                var wasAlreadyPublished = recipe.Status == "published";
                recipe.Status = "published";
                recipe.CurrentVersionId = versionToPublish.VersionId;
                recipe.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                if (wasAlreadyPublished)
                {
                    await _notificationService.NotifyRecipeUpdatedAsync(recipeId, recipe.Title, tenantId, publishedBy);
                }
                else
                {
                    await _notificationService.NotifyRecipePublishedAsync(recipeId, recipe.Title, tenantId, publishedBy);
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
                        VersionId = versionToPublish.VersionId,
                        VersionNumber = versionToPublish.VersionNumber,
                    }));
                await transaction.CommitAsync();
                return recipe;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            if (request.IsRatioAnchor != null) ingredient.IsRatioAnchor = request.IsRatioAnchor.Value;

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

        public async Task<RecipeVersion?> GetDraftVersionAsync(Guid recipeId, Guid tenantId)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId);
            if (recipe == null) return null;

            return await _context.RecipeVersions
                .Include(rv => rv.Steps)
                .Include(rv => rv.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(rv => rv.Ingredients)
                    .ThenInclude(ri => ri.UnitType)
                .Include(rv => rv.IngredientGroups)
                .FirstOrDefaultAsync(rv => rv.RecipeId == recipeId && rv.IsDraft);
        }

        public async Task<RecipeVersion> CreateDraftFromCurrentAsync(
            Guid recipeId, 
            Guid tenantId,
            Guid createdBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");
            var existingDraft = await _context.RecipeVersions
                .FirstOrDefaultAsync(rv => rv.RecipeId == recipeId && rv.IsDraft);

            if (existingDraft != null)
                throw new InvalidOperationException("A draft version already exists for this recipe.");

            var currentVersion = await _context.RecipeVersions
                .Include(rv => rv.Steps)
                .Include(rv => rv.Ingredients)
                .Include(rv => rv.IngredientGroups)
                .FirstOrDefaultAsync(rv => rv.VersionId == recipe.CurrentVersionId)
                ?? throw new KeyNotFoundException("Current version not found.");

            var nextVersionNumber = await _context.RecipeVersions
                .Where(rv => rv.RecipeId == recipeId)
                .MaxAsync(rv => rv.VersionNumber) + 1;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var draft = new RecipeVersion
                {
                    VersionId = Guid.NewGuid(),
                    RecipeId = recipeId,
                    VersionNumber = nextVersionNumber,
                    IsDraft = true,
                    IsPublished = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.RecipeVersions.AddAsync(draft);
                await _context.SaveChangesAsync();

                var groupIdMap = new Dictionary<Guid, Guid>();
                foreach (var group in currentVersion.IngredientGroups)
                {
                    var newGroupId = Guid.NewGuid();
                    groupIdMap[group.GroupId] = newGroupId;
                    await _context.RecipeIngredientGroups.AddAsync(new RecipeIngredientGroup
                    {
                        GroupId = newGroupId,
                        VersionId = draft.VersionId,
                        Name = group.Name,
                        DisplayOrder = group.DisplayOrder
                    });
                }

                foreach (var ingredient in currentVersion.Ingredients)
                {
                    await _context.RecipeIngredients.AddAsync(new RecipeIngredient
                    {
                        RecipeIngredientId = Guid.NewGuid(),
                        VersionId = draft.VersionId,
                        IngredientId = ingredient.IngredientId,
                        Quantity = ingredient.Quantity,
                        UnitTypeId = ingredient.UnitTypeId,
                        DisplayOrder = ingredient.DisplayOrder,
                        GroupId = ingredient.GroupId.HasValue && groupIdMap.ContainsKey(ingredient.GroupId.Value)
                            ? groupIdMap[ingredient.GroupId.Value]
                            : null,
                        IsNonConvertible = ingredient.IsNonConvertible,
                        IsRatioAnchor = ingredient.IsRatioAnchor
                    });
                }

                foreach (var step in currentVersion.Steps)
                {
                    await _context.RecipeSteps.AddAsync(new RecipeStep
                    {
                        StepId = Guid.NewGuid(),
                        VersionId = draft.VersionId,
                        StepNumber = step.StepNumber,
                        Instruction = step.Instruction,
                        HasTimer = step.HasTimer,
                        TimerDuration = step.TimerDuration,
                        IsAsync = step.IsAsync,
                        AsyncGroupId = step.AsyncGroupId,
                        CreatedAt = step.CreatedAt
                    });
                        

                }

                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create_draft",
                    "recipe",
                    recipeId,
                    null,
                    JsonSerializer.Serialize(new { VersionNumber = nextVersionNumber }));

                await transaction.CommitAsync();
                return draft;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Recipe> SaveDraftAsync(
            Guid recipeId,
            Guid versionId,
            SaveDraftRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var version = await _context.RecipeVersions
                .Include(rv => rv.Steps)
                .Include(rv => rv.Ingredients)
                .Include(rv => rv.IngredientGroups)
                .FirstOrDefaultAsync(rv => rv.VersionId == versionId && rv.RecipeId == recipeId)
                ?? throw new KeyNotFoundException("Draft version not found.");

            if (!version.IsDraft)
                throw new InvalidOperationException("Cannot save to a published version.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.RecipeIngredientGroups.RemoveRange(version.IngredientGroups);
                await _context.SaveChangesAsync();

                var groupIdMap = new Dictionary<Guid, Guid>();
                foreach (var group in request.IngredientGroups)
                {
                    var newGroupId = group.GroupId ?? Guid.NewGuid();
                    if (group.GroupId.HasValue) groupIdMap[group.GroupId.Value] = newGroupId;

                    await _context.RecipeIngredientGroups.AddAsync(new RecipeIngredientGroup
                    {
                        GroupId = newGroupId,
                        VersionId = versionId,
                        Name = group.Name,
                        DisplayOrder = group.DisplayOrder
                    });
                }

                Guid? currentGroupId = null;
                foreach (var step in request.Steps)
                {
                    if (step.IsAsync)
                    {
                        if (currentGroupId == null)
                            currentGroupId = Guid.NewGuid();
                        step.AsyncGroupId = currentGroupId;
                    }
                    else
                    {
                        currentGroupId = null;
                        step.AsyncGroupId = null;
                    }
                }

                _context.RecipeSteps.RemoveRange(version.Steps);
                await _context.SaveChangesAsync();

                _context.RecipeIngredients.RemoveRange(version.Ingredients);
                await _context.SaveChangesAsync();

                foreach (var ing in request.Ingredients)
                {
                    await _context.RecipeIngredients.AddAsync(new RecipeIngredient
                    {
                        RecipeIngredientId = ing.RecipeIngredientId ?? Guid.NewGuid(),
                        VersionId = versionId,
                        IngredientId = ing.IngredientId,
                        Quantity = ing.Quantity,
                        UnitTypeId = ing.UnitTypeId,
                        DisplayOrder = ing.DisplayOrder,
                        GroupId = ing.GroupId.HasValue && groupIdMap.ContainsKey(ing.GroupId.Value)
                            ? groupIdMap[ing.GroupId.Value]
                            : ing.GroupId,
                        IsNonConvertible = ing.IsNonConvertible,
                        IsRatioAnchor = ing.IsRatioAnchor
                    });
                }

                
                await _context.SaveChangesAsync();

                foreach (var step in request.Steps)
                {
                    await _context.RecipeSteps.AddAsync(new RecipeStep
                    {
                        StepId = step.StepId ?? Guid.NewGuid(),
                        VersionId = versionId,
                        StepNumber = step.StepNumber,
                        Instruction = step.Instruction,
                        HasTimer = step.HasTimer,
                        TimerDuration = step.TimerDuration,
                        IsAsync = step.IsAsync,
                        AsyncGroupId = step.AsyncGroupId,
                        CreatedAt = DateTime.UtcNow
                    });

                }

                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "save_draft",
                    "recipe",
                    recipeId,
                    null,
                    JsonSerializer.Serialize(new { versionId }));

                await transaction.CommitAsync();
                return recipe;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Recipe> RestoreVersionAsync(
            Guid recipeId,
            Guid versionId,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"Recipe {recipeId} not found.");

            var version = await _context.RecipeVersions
                .FirstOrDefaultAsync(rv => rv.VersionId == versionId && rv.RecipeId == recipeId)
                ?? throw new KeyNotFoundException("version not found.");

            if (version.IsDraft)
                throw new InvalidOperationException("Cannot restore a draft version.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingDraft = await _context.RecipeVersions
                    .Include(rv => rv.Steps)
                    .Include(rv => rv.Ingredients)
                    .Include(rv => rv.IngredientGroups)
                    .FirstOrDefaultAsync(rv => rv.RecipeId == recipeId && rv.IsDraft);

                if (existingDraft != null)
                {
                    _context.RecipeSteps.RemoveRange(existingDraft.Steps);
                    _context.RecipeIngredients.RemoveRange(existingDraft.Ingredients);
                    _context.RecipeIngredientGroups.RemoveRange(existingDraft.IngredientGroups);
                    _context.RecipeVersions.Remove(existingDraft);
                    await _context.SaveChangesAsync();
                }

                var previousVersionId = recipe.CurrentVersionId;
                recipe.CurrentVersionId = versionId;
                recipe.UpdatedAt = DateTime.UtcNow;
                await _recipeRepository.UpdateAsync(recipe);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "restore_version",
                    "recipe",
                    recipeId,
                    JsonSerializer.Serialize(new { VersionId = previousVersionId }),
                    JsonSerializer.Serialize(new { VersionId = versionId }));

                await transaction.CommitAsync();
                return recipe;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DiscardDraftAsync(
            Guid recipeId,
            Guid tenantId,
            Guid performedBy)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId)
                ?? throw new KeyNotFoundException($"REcipe {recipeId} not found.");

            var draft = await _context.RecipeVersions
                .Include(rv => rv.Steps)
                .Include(rv => rv.Ingredients)
                .Include(rv => rv.IngredientGroups)
                .FirstOrDefaultAsync(rv => rv.RecipeId == recipeId && rv.IsDraft)
                ?? throw new KeyNotFoundException("No draft version found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.RecipeSteps.RemoveRange(draft.Steps);
                _context.RecipeIngredients.RemoveRange(draft.Ingredients);
                _context.RecipeIngredientGroups.RemoveRange(draft.IngredientGroups);
                _context.RecipeVersions.Remove(draft);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "discard_draft",
                    "recipe",
                    recipeId,
                    null,
                    null);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<RecipeVersion>> GetVersionHistoryAsync(
            Guid recipeId,
            Guid tenantId)
        {
            var recipe = await _recipeRepository.GetByIdAndTenantAsync(recipeId, tenantId);
            if (recipe == null) return Enumerable.Empty<RecipeVersion>();

            return await _context.RecipeVersions
                .Where(rv => rv.RecipeId == recipeId && rv.IsPublished)
                .Include(rv => rv.PublishedByUser)
                .OrderByDescending(rv => rv.VersionNumber)
                .ToListAsync();
        }
    
    
    
    }

    
}
