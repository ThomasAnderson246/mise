using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IUnitTypeRepository : ITenantRepository<UnitType>
    {
        Task<IEnumerable<UnitType>> GetByMeasureTypeAsync(Guid tenantId, string measureType);
        Task<IEnumerable<UnitType>> GetBySystemAsync(Guid tenantId, string system);
        Task<bool> NameExistsInTenantAsync(Guid tenantId, string name);
    }
}
