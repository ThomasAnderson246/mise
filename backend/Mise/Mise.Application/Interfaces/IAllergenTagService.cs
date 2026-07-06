using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IAllergenTagService
    {
        Task<IEnumerable<AllergenTag>> GetAllAsync(Guid tenantId);
        Task<AllergenTag?> GetByIdAsync(Guid allergenId, Guid tenantId);
        Task<AllergenTag> CreateAsync(CreateAllergenTagRequest request, Guid tenantId, Guid createdBy);
        Task<AllergenTag> UpdateAsync(Guid allergenId, UpdateAllergenTagRequest request, Guid tenantId, Guid performedby);
        Task DeleteAsync(Guid allergenId, Guid tenantId, Guid performedBy);
    }
}
