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
    public class TenantRepository : BaseRepository<Tenant>, ITenantRepositoryService
    {
        public TenantRepository(MiseDbContext context) : base(context) { }

        public async Task<Tenant?> GetBySlugAsync(string slug)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<IEnumerable<Tenant>> GetAllActiveAsync()
        {
            return await _context.Tenants
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Tenants
                .AnyAsync(t => t.Slug == slug);
        }
    }
}
