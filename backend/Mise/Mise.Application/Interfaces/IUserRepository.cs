using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IUserRepository : ITenantRepository<User>
    {
        Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId);
        Task<IEnumerable<User>> GetByStatusAsync(Guid tenantId, string status);
        Task<bool> EmailExistsInTenantAsync(string email, Guid tenantId);
    }
}
