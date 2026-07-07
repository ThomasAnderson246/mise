using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface ICategoryRepository : ITenantRepository<Category>
    {
        Task<bool> NameExistsInTenantAsync(Guid tenantId, string name);
    }
}
