using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IAllergenTagRepository : ITenantRepository<AllergenTag>
    {
        Task<IEnumerable<AllergenTag>> GetSystemDefinedAsync(Guid tenantId);
        Task<bool> NameExistsInTenantAsync(Guid tenantId, string name);
    }
}
