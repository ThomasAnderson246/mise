using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync(Guid tenantId);
        Task<Role?> GetByIdAsync(Guid roleId, Guid tenantId);
        Task<Role> CreateAsync(CreateRoleRequest request, Guid tenantId, Guid createBy);
        Task<Role> UpdateAsync(Guid roleId, UpdateRoleRequest request, Guid tenantId, Guid performedBy);
        Task DeleteAsync(Guid roleId, Guid tenantId, Guid performedBy);
        Task AssignPermissionAsync(Guid roleId, Guid permissionsId, Guid tenantId, Guid performedBy);
        Task RemovePermissionAsync(Guid roleId, Guid permissionsId, Guid tenantId, Guid performedBy);
    }
}
