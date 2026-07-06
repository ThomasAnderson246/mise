using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Persistence.Repositories
{
    public class AllergenTagRepository : BaseTenantRepository<AllergenTag>, IAllergenTagRepository
    {

        public AllergenTagRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<AllergenTag>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.AllergenTags
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.IsSystemDefined)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }

        public override async Task<AllergenTag?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.AllergenTags
                .FirstOrDefaultAsync(a => a.AllergenId == id && a.TenantId == tenantId);
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.AllergenTags
                .AnyAsync(a => a.AllergenId == id && a.TenantId == tenantId);
        }

        public async Task<IEnumerable<AllergenTag>> GetSystemDefinedAsync(Guid tenantId)
        {
            return await _context.AllergenTags
                .Where(a => a.TenantId == tenantId && a.IsSystemDefined)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<bool> NameExistsInTenantAsync(Guid tenantId, string name)
        {
            return await _context.AllergenTags
                .AnyAsync(a => a.TenantId == tenantId && a.Name.ToLower() == name.ToLower());
        }
    }
}
