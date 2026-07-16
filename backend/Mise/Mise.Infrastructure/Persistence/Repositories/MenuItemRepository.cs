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
    public class MenuItemRepository : BaseTenantRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<MenuItem>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.MenuItems
                .Where(mi => mi.TenantId == tenantId)
                .Include(mi => mi.MenuItemRecipes)
                    .ThenInclude(mir => mir.Recipe)
                .Include(mi => mi.MenuItemAllergens)
                    .ThenInclude(mia => mia.AllergenTag)
                .OrderBy(mi => mi.Course)
                .ThenBy(mi => mi.Name)
                .ToListAsync();
        }

        public override async Task<MenuItem?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.MenuItems
                .Where(mi => mi.MenuItemId == id && mi.TenantId == tenantId)
                .Include(mi => mi.MenuItemRecipes)
                .ThenInclude(mir => mir.Recipe)
                .Include(mi => mi.MenuItemAllergens)
                    .ThenInclude(mia => mia.AllergenTag)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.MenuItems
                .AnyAsync(mi => mi.MenuItemId == id && mi.TenantId == tenantId);
        }

        public async Task<IEnumerable<MenuItem>> GetByCourseAsync(Guid tenantId, string course)
        {
            return await _context.MenuItems
                .Where(mi => mi.TenantId == tenantId && mi.Course == course)
                .Include(mi => mi.MenuItemRecipes)
                    .ThenInclude(mir => mir.Recipe)
                .Include(mi => mi.MenuItemAllergens)
                    .ThenInclude(mia => mia.AllergenTag)
                .OrderBy(mi => mi.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByStatusAsync(Guid tenantId, string status)
        {
            return await _context.MenuItems
                .Where(mi => mi.TenantId == tenantId && mi.Status == status)
                .Include(mi => mi.MenuItemRecipes)
                    .ThenInclude(mir => mir.Recipe)
                .Include(mi => mi.MenuItemAllergens)
                    .ThenInclude(mia => mia.AllergenTag)
                .OrderBy(mi => mi.Course)
                .ThenBy(mi => mi.Name)
                .ToListAsync();
        }

        public async Task<MenuItem?> GetWithFullDetailsAsync(Guid menuItemId, Guid tenantId)
        {
            return await _context.MenuItems
                .Where(mi => mi.MenuItemId == menuItemId && mi.TenantId == tenantId)
                .Include(mi => mi.MenuItemRecipes.OrderBy(mir => mir.DisplayOrder))
                    .ThenInclude(mir => mir.Recipe)
                        .ThenInclude(r => r.CurrentVersion)
                            .ThenInclude(rv => rv!.Ingredients)
                                .ThenInclude(ri => ri.Ingredient)
                                    .ThenInclude(i => i.IngredientAllergens)
                                        .ThenInclude(ia => ia.AllergenTag)
                .Include(mi => mi.MenuItemAllergens)
                    .ThenInclude(mia => mia.AllergenTag)
                .Include(mi => mi.CreatedByUser)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> NameExistsInTenantAsync(Guid tenantId, string name)
        {
            return await _context.MenuItems
                .AnyAsync(mi => mi.TenantId == tenantId && mi.Name.ToLower() == name.ToLower());
        }
    }
}
