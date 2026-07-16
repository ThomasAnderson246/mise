using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IPrepListService
    {
        Task<IEnumerable<PrepList>> GetAllAsync(Guid tenantId);
        Task<PrepList?> GetByIdAsync(Guid prepListId, Guid tenantId);
        Task<IEnumerable<PrepList>> GetByStatusAsync(Guid tenantId, bool isComplete);
        Task<PrepList> CreateAsync(CreatePrepListRequest request, Guid tenantId, Guid createdBy);
        Task DeleteAsync(Guid prepListId, Guid tenantId, Guid performedBy);
        Task<PrepList> AddItemAsync(Guid prepListId, AddPrepListItemRequest request, Guid tenantId, Guid performedBy);
        Task<PrepList> UpdateItemAsync(Guid prepListId, Guid itemId, UpdatePrepListItemRequest request, Guid tenantId, Guid performedBy);
        Task<PrepList> RemoveItemAsync(Guid prepListId, Guid itemId, Guid tenantId, Guid performedBy);
        Task<PrepList> CompleteItemAsync(Guid prepLIstId, Guid itemId, Guid tenantId, Guid completedBy);
        Task<PrepList> CompletePrepListAsync(Guid prepListId, Guid tenantId, Guid completedBy);
    }
}
