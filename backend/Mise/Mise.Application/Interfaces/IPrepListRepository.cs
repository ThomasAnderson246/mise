using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IPrepListRepository : ITenantRepository<PrepList>
    {
        Task<IEnumerable<PrepList>> GetByStatusAsync(Guid tenantId, bool isComplete);
        Task<PrepList?> GetWithItemsAsync(Guid prepListId, Guid tenantId);
        
    }
}
