using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItem>> GetAllAsync(Guid tenantId);
        Task<MenuItem?> GetByIdAsync(Guid menuItemId, Guid tenantId);
        Task<IEnumerable<MenuItem>> GetByCourseAsync(Guid tenantId, string course);
        Task<IEnumerable<MenuItem>> GetByStatusAsync(Guid tenantId, string status);
        Task<MenuItem> CreateAsync(CreateMenuItemRequest request, Guid tenantId, Guid createdBy);
        Task<MenuItem> UpdateAsync(Guid enuItemId, UpdateMenuItemRequest request, Guid tenantId, Guid performedBy);
        Task DeleteAsync(Guid menuItemId, Guid tenantId, Guid performedBy);
        Task<MenuItem> PublishAsync(Guid menuItemId, Guid tenantId, Guid performedBy);
        Task<MenuItem> AddRecipeAsync(Guid menuItemId, AddMenuItemRecipeRequest request, Guid tenantId, Guid performedBy);
        Task<MenuItem> RemoveRecipeAsync(Guid menuItemId, Guid recipeId, Guid tenantId, Guid performedBy);
        Task<MenuItem> ResolveAllergensAsync(Guid menuItemId, AddMenuItemAllergenRequest request, Guid tenantId, Guid performedBy);
        Task<MenuItem> AddManualAllergenAsync(Guid menuItemId, AddMenuItemAllergenRequest request, Guid tenantId, Guid performeDby);
        Task<MenuItem> RemoveManualAllergenAsync(Guid menuItemId, Guid menuItemAllergenId, Guid tenantId, Guid performedBy);

    }
}
