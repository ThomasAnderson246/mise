using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IUnitTypeService
    {
        Task<IEnumerable<UnitType>> GetAllAsync(Guid tenantId);
        Task<UnitType?> GetByIdAsync(Guid unitTypeId, Guid tenantId);
        Task<IEnumerable<UnitType>> GetByMeasureTypeAsync(Guid tenantId, string measureType);
        Task<IEnumerable<UnitType>> GetBySystemAsync(Guid tenantId, string systemType);
        Task<UnitType> CreateAsync(CreateUnitTypeRequest request, Guid tenantId, Guid performedBy);
        Task<UnitType> UpdateAsync(Guid unitTypeId, UpdateUnityTypeRequest request, Guid tenantId, Guid performedBy);
        Task DeleteAsync(Guid unitTypId, Guid tenantId, Guid performedBy);
    }
}
