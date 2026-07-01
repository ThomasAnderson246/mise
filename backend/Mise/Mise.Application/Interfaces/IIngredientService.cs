using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllAsync(Guid tenantId);
        Task<Ingredient?> GetByIdAsync(Guid ingredientId, Guid tenantId);
        Task<IEnumerable<Ingredient>> SearchAsync(Guid dtenantId, string searchTerm);
        Task<Ingredient> CreateAsync(CreateIngredientRequest request, Guid tenantId, Guid createdBy);
        Task<Ingredient> UpdateAsync(Guid ingredientId, UpdateIngredientRequest request, Guid tenantId, Guid performedBy);
        Task DeleteAsync(Guid ingredientId, Guid tenantId, Guid performedBy);
    }
}
