using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class SubRecipeService : ISubRecipeService
    {

        private readonly MiseDbContext _context;
        private readonly IAuditLogServices _auditLogServices;

        public SubRecipeService(MiseDbContext context, IAuditLogServices auditLogServices)
        {
            _context = context;
            _auditLogServices = auditLogServices;
        }

        public async Task<IEnumerable<SubRecipe>> GetByParentAsync(Guid parentRecipeId, Guid tenantId)
        {
            return await _context.SubRecipes
                .Where(sr => sr.ParentRecipeId == parentRecipeId && sr.ParentRecipe.TenantId == tenantId)
                .Include(sr => sr.ChildRecipe)
                .ToListAsync();
        }

        public async Task AddAsync(
            Guid parentRecipeId,
            Guid subRecipeId,
            Guid tenantId,
            Guid performedBy)
        {
            if (parentRecipeId == subRecipeId)
                throw new InvalidOperationException("A recipe cannot be a sub-recipe of itself.");

            var parentExists = await _context.Recipes
                .AnyAsync(r => r.RecipeId == parentRecipeId && r.TenantId == tenantId);
            if (!parentExists)
                throw new KeyNotFoundException($"Parent recipe {parentRecipeId} does not exist.");

            var childExists = await _context.Recipes
                .AnyAsync(r => r.RecipeId == subRecipeId && r.TenantId == tenantId);
            if (!childExists)
                throw new KeyNotFoundException($"Sub-recipe {subRecipeId} not found.");

            var alreadyLinked = await _context.SubRecipes.AnyAsync(
                sr => sr.ParentRecipeId == parentRecipeId && sr.SubRecipeId == subRecipeId);
            if (alreadyLinked)
                throw new InvalidOperationException("This recipe is already a sub-recipe of the parent.");

            // check for circular reference
            var wouldCreateCircle = await _context.SubRecipes
                .AnyAsync(sr => sr.ParentRecipeId == subRecipeId && sr.SubRecipeId == parentRecipeId);
            if (wouldCreateCircle)
                throw new InvalidOperationException("Adding this sub-recipe would create a circular reference.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subRecipe = new SubRecipe
                {
                    ParentRecipeId = parentRecipeId,
                    SubRecipeId = subRecipeId
                };

                await _context.SubRecipes.AddAsync(subRecipe);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "add_sub_recipe",
                    "recipe",
                    parentRecipeId,
                    null,
                    JsonSerializer.Serialize(new { SubRecipeId = subRecipeId }));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveAsync(
            Guid parentRecipeId,
            Guid subRecipeId,
            Guid tenantId,
            Guid performedBy)
        {
            var subRecipe = await _context.SubRecipes
                .FirstOrDefaultAsync(sr => sr.ParentRecipeId == parentRecipeId && sr.SubRecipeId == subRecipeId)
                ?? throw new KeyNotFoundException("Sub-recipe relationship not found.");

            var parentExists = await _context.Recipes
                .AnyAsync(r => r.RecipeId == parentRecipeId && r.TenantId == tenantId);

            if (!parentExists)
                throw new KeyNotFoundException($"Parent recipe {parentRecipeId} not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.SubRecipes.Remove(subRecipe);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "remove_sub_recipe",
                    "recipe",
                    parentRecipeId,
                    JsonSerializer.Serialize(new { SubRecipeId = subRecipeId }),
                    null);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
