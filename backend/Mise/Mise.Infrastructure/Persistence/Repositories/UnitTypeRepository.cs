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
    public class UnitTypeRepository : BaseTenantRepository<UnitType>, IUnitTypeRepository
    {
        public UnitTypeRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<UnitType>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.UnitTypes
                .Where(u => u.TenantId == tenantId)
                .OrderBy(u => u.MeasureType)
                .ThenBy(u => u.System)
                .ThenBy(u => u.Name)
                .ToListAsync();
        }

        public override async Task<UnitType?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.UnitTypes
                .FirstOrDefaultAsync(u => u.UnitTypeId == id && u.TenantId == tenantId);
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.UnitTypes
                .AnyAsync(u => u.UnitTypeId == id && u.TenantId == tenantId);
        }

        public async Task<IEnumerable<UnitType>> GetByMeasureTypeAsync(Guid tenantId, string measureType)
        {
            return await _context.UnitTypes
                .Where(u => u.TenantId == tenantId && u.MeasureType == measureType)
                .OrderBy(u => u.System)
                .ThenBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<UnitType>> GetBySystemAsync(Guid tenantId, string system)
        {
            return await _context.UnitTypes
                .Where(u => u.TenantId == tenantId && u.System == system)
                .OrderBy(u => u.MeasureType)
                .ThenBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<bool> NameExistsInTenantAsync(Guid tenantId, string name)
        {
            return await _context.UnitTypes
                .AnyAsync(u => u.TenantId == tenantId && u.Name.ToLower() == name.ToLower());
        }
    }
}
