using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;


namespace Mise.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IAuditLogServices _auditLogServices;

        public CategoryService(ICategoryRepository categoryRepository, IAuditLogServices auditLogServices)
        {
            _categoryRepository = categoryRepository;
            _auditLogServices = auditLogServices;
        }

        public async Task<IEnumerable<Category>> GetallAsync(Guid tenantId)
        {
            return await _categoryRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<Category?> GetbyIdAsync(Guid categoryId, Guid tenantId)
        {
            return await _categoryRepository.GetByIdAndTenantAsync(categoryId, tenantId);
        }

        public async Task<Category> CreateAsync(
            CreateCategoryRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            var nameExists = await _categoryRepository.NameExistsInTenantAsync(tenantId, request.Name);
            if (nameExists)
                throw new InvalidOperationException($"A category with the name '{request.Name}' already exists.");

            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                TenantId = tenantId,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);

            await _auditLogServices.LogAsync(
                tenantId,
                createdBy,
                "create",
                "category",
                category.CategoryId,
                null,
                JsonSerializer.Serialize(new
                {
                    category.Name,
                    category.Description,
                }));

            return category;
        }

        public async Task<Category> UpdateAsync(
            Guid categoryId, 
            UpdateCategoryRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var category = await _categoryRepository.GetByIdAndTenantAsync(categoryId, tenantId)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (request.Name != null)
            {
                var nameExists = await _categoryRepository.NameExistsInTenantAsync(tenantId, request.Name);
                if (nameExists && request.Name.ToLower() != category.Name.ToLower())
                    throw new InvalidOperationException($"A category with the name '{request.Name}' already exists.");
            }

            var previousState = JsonSerializer.Serialize(new
            {
                category.Name,
                category.Description,
            });

            if (request.Name != null) category.Name = request.Name;
            if (request.Description != null) category.Description = request.Description;

            await _categoryRepository.UpdateAsync(category);

            var newState = JsonSerializer.Serialize(new
            {
                category.Name,
                category.Description
            });

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "update",
                "category",
                category.CategoryId,
                previousState,
                newState);

            return category;
        }

        public async Task DeleteAsync(Guid categoryId, Guid tenantId, Guid performedBy)
        {
            var category = await _categoryRepository.GetByIdAndTenantAsync(categoryId, tenantId)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            var previousState = JsonSerializer.Serialize(new
            {
                category.Name,
                category.Description,
            });

            await _categoryRepository.DeleteAsync(categoryId);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "delete",
                "category",
                categoryId,
                previousState,
                null
                );
        }
    }
}
