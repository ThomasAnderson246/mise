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
    public class CategoryRepository : BaseTenantRepository<Category>, ICategoryRepository
    {

        public CategoryRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<Category>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public override async Task<Category?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.TenantId == tenantId);
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryId == id && c.TenantId == tenantId);
        }

        public async Task<bool> NameExistsInTenantAsync(Guid tenantId, string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.TenantId == tenantId && c.Name.ToLower() == name.ToLower());
        }
    }
}
