using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllAsync(Guid tenantId);
        Task<Recipe?> GetByIdAsync(Guid recipeId, Guid tenantId);
        Task<Recipe> CreateAsync(CreateRecipeRequest request, Guid tenantId, Guid createdBy);
        Task<Recipe> UpdateAsync(Guid recipeId, UpdateRecipeRequest request, Guid tenantId, Guid performedBy);
        Task DeleteAsync(Guid recipeId, Guid tenantId, Guid performedBy);
        Task<Recipe> PublishAsync(Guid recipeId, Guid tenantId, Guid publishedBy);

    }
}
