using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface ISubRecipeService
    {
        Task<IEnumerable<SubRecipe>> GetByParentAsync(Guid parentRecipeId, Guid tenantId);
        Task AddAsync(Guid parentRecipeId, Guid subRecipeId, Guid tenantId, Guid performedBy);
        Task RemoveAsync(Guid parentRecipeId, Guid subRecipeId, Guid tenantId, Guid performedBy);
    }
}
