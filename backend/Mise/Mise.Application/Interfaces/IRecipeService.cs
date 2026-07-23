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
        Task<Recipe> AddIngredientAsync(Guid recipeId, AddRecipeIngredientRequest request, Guid tenantId, Guid performedBy);
        Task<Recipe> UpdateIngredientAsync(Guid recipeId, Guid recipeIngredientId, UpdateRecipeIngredientRequest request, Guid tenantId, Guid performedBy);
        Task<Recipe> RemoveIngredientAsync(Guid recipeId, Guid recipeIngredientId, Guid tenantId, Guid performedBy);
        Task<Recipe> AddStepAsync(Guid recipeId, AddRecipeStepRequest request, Guid tenantId, Guid performedBy);
        Task<Recipe> UpdateStepAsync(Guid recipeId, Guid stepId, UpdateRecipeStepRequest request, Guid tenantId, Guid performedBy);
        Task<Recipe> RemoveStepAsync(Guid recipeId, Guid stepId, Guid tenantId, Guid performedBy);
        Task<Recipe> AddIngredientGroupAsync(Guid recipeId, AddRecipeIngredientGroupRequest request, Guid tenantId, Guid performedBy);
        Task<Recipe> RemoveIngredientGroupAsync(Guid recipeId, Guid groupId, Guid tenantId, Guid performedBy);

    }
}
