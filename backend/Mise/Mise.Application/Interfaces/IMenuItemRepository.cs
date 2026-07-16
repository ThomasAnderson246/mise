using Mise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.Interfaces
{
    public interface IMenuItemRepository : ITenantRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetByCourseAsync(Guid tenantId, string course);
        Task<IEnumerable<MenuItem>> GetByStatusAsync(Guid teantId, string status);
        Task<MenuItem?> GetWithFullDetailsAsync(Guid menuItemId, Guid tenantId);
        Task<bool> NameExistsInTenantAsync(Guid teantnId, string name);
    }
}
