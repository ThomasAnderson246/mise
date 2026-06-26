using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Persistence.Repositories
{
    public class RecipeRepository : BaseTenantRepository<Recipe>
    {

        public RecipeRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<Recipe>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.Recipes
                .Where(r => r.TenantId == tenantId)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.CurrentVersion)
                .OrderBy(r => r.Title)
                .ToListAsync();
        }

        public override async Task<Recipe?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Recipes
                .Where(r => r.RecipeId == id && r.TenantId == tenantId)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.CurrentVersion)
                    .ThenInclude(rv => rv!.Steps)
                .Include(r => r.CurrentVersion)
                    .ThenInclude(rv => rv!.Ingredients)
                        .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Recipes
                .AnyAsync(r => r.RecipeId == id && r.TenantId == tenantId);
        }

        public async Task<IEnumerable<Recipe>> GetByStatusAsync(Guid tenantId, string status)
        {
            return await _context.Recipes
                .Where(r => r.TenantId == tenantId && r.Status == status)
                .OrderBy(r => r.Title)
                .ToListAsync();
        }
    }
}
