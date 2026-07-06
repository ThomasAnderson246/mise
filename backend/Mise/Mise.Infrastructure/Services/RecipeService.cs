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

namespace Mise.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly MiseDbContext _context;
        private readonly IAuditLogServices _auditLogServices;

        public RecipeService(IRecipeRepository reciperRepository, MiseDbContext context, IAuditLogServices auditLogServices)
        {
            _recipeRepository = reciperRepository;
            _context = context;
            _auditLogServices = auditLogServices;
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

            recipe.Status = "published";
            recipe.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

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
    }
}
