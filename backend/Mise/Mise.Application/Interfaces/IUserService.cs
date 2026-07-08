using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync(Guid tenantId);
        Task<User?> GetByIdAsync(Guid userId, Guid tenantId);
        Task<(User user, string temporaryPassword)> InviteAsync(InviteUserRequest request, Guid tenantId, Guid invitedBy);
        Task<User> UpdateAsync(Guid userId, UpdateUserRequest request, Guid tenantId, Guid performedBy);
        Task DeactivateAsync(Guid userId, Guid tenantId, Guid performedBy);
        Task ReactivateAsync(Guid userId, Guid tenantId, Guid performedBy);
        Task AssignRoleAsync(Guid userId, Guid roleId, Guid tenantId, Guid assignedBy);
        Task RemoveRoleAsync(Guid userId, Guid roleId, Guid tenantId, Guid performedBy);

    }
}
