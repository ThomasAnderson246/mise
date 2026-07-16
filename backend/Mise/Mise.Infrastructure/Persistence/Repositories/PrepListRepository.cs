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
    public class PrepListRepository : BaseTenantRepository<PrepList>, IPrepListRepository
    {
        public PrepListRepository(MiseDbContext context) : base(context) { }    

        public override async Task<IEnumerable<PrepList>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.PrepLists
                .Where(pl => pl.TenantId == tenantId)
                .Include(pl => pl.Items)
                    .ThenInclude(i => i.Recipe)
                .Include(pl => pl.CreatedByUser)
                .OrderByDescending(pl => pl.CreatedAt)
                .ToListAsync();
        }

        public override async Task<PrepList?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.PrepLists
                .Where(pl => pl.PrepListId == id && pl.TenantId == tenantId)
                .Include(pl => pl.Items)
                    .ThenInclude(i => i.Recipe)
                .Include(pl => pl.CreatedByUser)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.PrepLists
                .AnyAsync(pl => pl.PrepListId == id && pl.TenantId == tenantId);
        }

        public async Task<IEnumerable<PrepList>> GetByStatusAsync(Guid tenantId, bool isComplete)
        {
            return await _context.PrepLists
                .Where(pl => pl.TenantId == tenantId && pl.IsComplete == isComplete)
                .Include(pl => pl.Items)
                .ThenInclude(i => i.Recipe)
                .OrderByDescending(pl => pl.CreatedAt)
                .ToListAsync();
        }

        public async Task<PrepList?> GetWithItemsAsync(Guid prepListId, Guid tenantId)
        {
            return await _context.PrepLists
                .Where(pl => pl.PrepListId == prepListId && pl.TenantId == tenantId)
                .Include(pl => pl.Items.OrderBy(i => i.DisplayOrder))
                    .ThenInclude(i => i.Recipe)
                .Include(pl => pl.Items)
                    .ThenInclude(i => i.CompletedByUser)
                .FirstOrDefaultAsync();
        }
    }
}
