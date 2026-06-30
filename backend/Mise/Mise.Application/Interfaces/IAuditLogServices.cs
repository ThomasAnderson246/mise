using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IAuditLogServices
    {
        Task LogAsync(
            Guid tenantId,
            Guid? performedBy,
            string action,
            string resource,
            Guid resourceId,
            string? previousState = null,
            string? newState = null,
            string? ipAddress = null);

        Task<IEnumerable<AuditLog>> GetByTenantAsync(Guid tenantId);
        Task<IEnumerable<AuditLog>> GetByResourceAsync(Guid tenantId, string resource, Guid resourceId);
    }
}
