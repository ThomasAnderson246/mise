using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class AuditLogService : IAuditLogServices
    {

        private readonly MiseDbContext _context;

        public AuditLogService(MiseDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            Guid tenantId,
            Guid? performedBy,
            string action, 
            string resource,
            Guid resourceId,
            string? previousState = null,
            string? newState = null,
            string? ipAddress = null)
        {
            var log = new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TenantId = tenantId,
                PerformedBy = performedBy,
                Action = action,
                Resource = resource,
                ResourceId = resourceId,
                PreviousState = previousState,
                NewState = newState,
                IpAddress = ipAddress,
                PerformedAt = DateTime.UtcNow,
            };

            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByTenantAsync(Guid tenantId)
        {
            return await _context.AuditLogs
                .Where(a => a.TenantId == tenantId)
                .Include(a => a.PerformedByUser)
                .OrderByDescending(a => a.PerformedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetbyResourceAsync(
            Guid tenantId,
            string resource,
            Guid resourceId)
        {
            return await _context.AuditLogs
                .Where(a => a.TenantId == tenantId && a.Resource == resource && a.ResourceId == resourceId)
                .Include(a => a.PerformedByUser)
                .OrderByDescending(a => a.PerformedAt)
                .ToListAsync();
        }

       public async Task<IEnumerable<AuditLog>> GetByResourceAsync(
           Guid tenantId,
           string resource,
           Guid resourceId)
        {
            return await _context.AuditLogs
                .Where(a => a.TenantId == tenantId && a.Resource == resource && a.ResourceId == resourceId)
                .Include(a => a.PerformedByUser)
                .OrderByDescending(a => a.PerformedAt)
                .ToListAsync();
        }
    }
}
