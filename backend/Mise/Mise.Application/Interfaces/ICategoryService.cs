using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetallAsync(Guid tenantId);
        Task<Category?> GetbyIdAsync(Guid categoryid, Guid tenantId);
        Task<Category> CreateAsync(CreateCategoryRequest request, Guid tenantId, Guid createdBy);
        Task<Category> UpdateAsync(Guid categoryid, UpdateCategoryRequest request, Guid tenantId, Guid performdBy);
        Task DeleteAsync(Guid categoryId, Guid tenantId, Guid performedBy);
    }
}
