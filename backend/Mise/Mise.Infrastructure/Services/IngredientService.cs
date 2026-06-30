using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class IngredientService : IIngredientService
    {

        private readonly IIngredientRepository _ingredientRepository;
        private readonly MiseDbContext _context;
        private readonly IAuditLogServices _auditLogServices;

        public IngredientService(IIngredientRepository ingredientRepository, MiseDbContext context, IAuditLogServices auditLogServices)
        {
            _ingredientRepository = ingredientRepository;
            _context = context;
            _auditLogServices = auditLogServices;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync(Guid tenantId)
        {
            return await _ingredientRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<Ingredient?> GetByIdAsync(Guid ingredientId, Guid tenantId)
        {
            return await _ingredientRepository.GetByIdAndTenantAsync(ingredientId, tenantId);
        }

        public async Task<IEnumerable<Ingredient>> SearchAsync(Guid tenantId, string searchTerm)
        {
            return await _ingredientRepository.SearchByNameAsync(tenantId, searchTerm);
        }

        public async Task<Ingredient> CreateAsync(
            CreateIngredientRequest request,
            Guid tenantId, 
            Guid createdBy)
        {
            var ingredient = new Ingredient
            {
                IngredientId = Guid.NewGuid(),
                TenantId = tenantId,
                Name = request.Name,
                Category = request.Category,
                DefaultUnitTypeId = request.DefaultUnitTypeId,
                IsNonConvertible = request.IsNonConvertible,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow

            };

            await _ingredientRepository.AddAsync(ingredient);

            if (request.AllergenIds != null && request.AllergenIds.Any())
            {
                var ingredientAllergens = request.AllergenIds.Select(aId => new IngredientAllergen
                {
                    IngredientId = ingredient.IngredientId,
                    AllergenId = aId
                }).ToList();

                await _context.IngredientAllergens.AddRangeAsync(ingredientAllergens);
                await _context.SaveChangesAsync();
            }

            await _auditLogServices.LogAsync(
                tenantId,
                createdBy,
                "create",
                "ingredient",
                ingredient.IngredientId,
                null,
                ingredient.Name);

            return ingredient;
        }

        public async Task<Ingredient> UpdateAsync(
            Guid ingredientId,
            UpdateIngredientRequest request,
            Guid tenantId)
        {
            var ingredient = await _ingredientRepository.GetByIdAndTenantAsync(ingredientId, tenantId)
                ?? throw new KeyNotFoundException($"Ingredient {ingredientId} not found.");

            var previousState = ingredient.Name;

            if (request.Name != null) ingredient.Name = request.Name;
            if (request.Category != null) ingredient.Category = request.Category;
            if (request.DefaultUnitTypeId != null) ingredient.DefaultUnitTypeId = request.DefaultUnitTypeId;
            if (request.IsNonConvertible != null) ingredient.IsNonConvertible = request.IsNonConvertible.Value;
            ingredient.UpdatedAt = DateTime.UtcNow;

            await _ingredientRepository.UpdateAsync(ingredient);

            if (request.AllergenIds != null)
            {
                var existing = _context.IngredientAllergens
                    .Where(ia => ia.IngredientId == ingredientId);
                _context.IngredientAllergens.RemoveRange(existing);

                var newAllergens = request.AllergenIds.Select(aId => new IngredientAllergen
                {
                    IngredientId = ingredientId,
                    AllergenId = aId
                }).ToList();

                await _context.IngredientAllergens.AddRangeAsync(newAllergens);
                await _context.SaveChangesAsync();
            }

            await _auditLogServices.LogAsync(
                tenantId,
                null,
                "update",
                "ingredient",
                ingredient.IngredientId,
                previousState,
                ingredient.Name);

            return ingredient;

        }

        public async Task DeleteAsync(Guid ingredientId, Guid tenantId)
        {
            var exists = await _ingredientRepository.ExistsInTenantAsync(ingredientId, tenantId);
            if (!exists)
                throw new KeyNotFoundException($"Ingredient {ingredientId} not found.");

            await _ingredientRepository.DeleteAsync(ingredientId);

            await _auditLogServices.LogAsync(
                tenantId,
                null,
                "delete",
                "ingredient",
                ingredientId);
        }
    }
}
