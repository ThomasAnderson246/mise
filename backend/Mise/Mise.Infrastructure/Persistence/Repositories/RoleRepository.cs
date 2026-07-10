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
    public class RoleRepository :  BaseTenantRepository<Role>, IRoleRepository
    {
        public RoleRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<Role>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.Roles
                .Where(r => r.TenantId == tenantId)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .OrderByDescending(r => r.IsSystemRole)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        public override async Task<Role?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Roles
                .Where(r => r.RoleId == id && r.TenantId == tenantId)
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Roles
                .AnyAsync(r => r.RoleId == id && r.TenantId == tenantId);
        }

        public async Task<bool> NameExistsInTenantAsync(Guid tenantId, string name)
        {
            return await _context.Roles
                .AnyAsync(r => r.TenantId == tenantId && r.Name.ToLower() == name.ToLower());
        }

        public async Task<Role?> GetWithPermissionsAsync(Guid roleId, Guid tenantId)
        {
            return await _context.Roles
                .Where(r => r.RoleId == roleId && r.TenantId == tenantId)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync();
        }
    }
}
