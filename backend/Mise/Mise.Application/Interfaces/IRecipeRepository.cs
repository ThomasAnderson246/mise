using Mise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.Interfaces
{
    public interface IRecipeRepository : ITenantRepository<Recipe>
    {
        Task<IEnumerable<Recipe>> GetByStatusAsync(Guid tenantId, string status);
    }
}
