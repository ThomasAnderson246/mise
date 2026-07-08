using Mise.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Mise.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseTenantRepository<User>, IUserRepository
    {

        public UserRepository(MiseDbContext context) : base(context) { }

        public override async Task<IEnumerable<User>> GetAllByTenantAsync(Guid tenantId)
        {
            return await _context.Users
                .Where(u => u.TenantId == tenantId)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.LastName)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Users
                .Where(u => u.UserId == id && u.TenantId == tenantId)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync();
        }

        public override async Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == id && u.TenantId == tenantId);
        }

        public async Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
        }

        public async Task<IEnumerable<User>> GetByStatusAsync(Guid tenantId, string status)
        {
            return await _context.Users
                .Where(u => u.TenantId == tenantId && u.Status == status)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsInTenantAsync(string email, Guid tenantId)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email && u.TenantId == tenantId);
        }
    }
}
