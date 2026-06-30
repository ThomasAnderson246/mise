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
    public class IngredientRepository : BaseTenantRepository<Ingredient>, IIngredientRepository
    {

        public IngredientRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<Ingredient>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.Ingredients
                .Where(i => i.TenantId == tenantId)
                .Include(i => i.IngredientAllergens)
                    .ThenInclude(ia => ia.AllergenTag)
                .Include(i => i.DefaultUnitType)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public override async Task<Ingredient?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Ingredients
                .Where(i => i.IngredientId == id && i.TenantId == tenantId)
                .Include(i => i.IngredientAllergens)
                    .ThenInclude(ia => ia.AllergenTag)
                .Include(i => i.DefaultUnitType)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Ingredients
                .AnyAsync(i => i.IngredientId == id && i.TenantId == tenantId);
        }

        public async Task<IEnumerable<Ingredient>> SearchByNameAsync(Guid tenantId, string searchTerm)
        {
            return await _context.Ingredients
                .Where(i => i.TenantId == tenantId && i.Name.ToLower().Contains(searchTerm.ToLower()))
                .Include(i => i.IngredientAllergens)
                    .ThenInclude(ia => ia.AllergenTag)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }
    }
}
