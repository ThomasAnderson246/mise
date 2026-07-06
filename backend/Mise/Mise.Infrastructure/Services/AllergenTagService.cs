using Mise.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Mise.Application.DTOs;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Services
{
    public class AllergenTagService : IAllergenTagService
    {
        private readonly IAllergenTagRepository _allergenTagRepository;
        private readonly IAuditLogServices _auditLogServices;

        public AllergenTagService(
            IAllergenTagRepository allergenTagRepository, IAuditLogServices auditLogServices)
        {
            _allergenTagRepository = allergenTagRepository;
            _auditLogServices = auditLogServices;
        }

        public async Task<IEnumerable<AllergenTag>> GetAllAsync(Guid tenantId)
        {
            return await _allergenTagRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<AllergenTag?> GetByIdAsync(Guid allergenId, Guid tenantId)
        {
            return await _allergenTagRepository.GetByIdAndTenantAsync(allergenId, tenantId);
        }

        public async Task<AllergenTag> CreateAsync(
            CreateAllergenTagRequest request, 
            Guid tenantId,
            Guid createdBy)
        {
            var nameExists = await _allergenTagRepository.NameExistsInTenantAsync(tenantId, request.Name);

            if (nameExists)
                throw new InvalidOperationException($"Ana allergen tag with that name already exists.");

            var allergenTag = new AllergenTag
            {
                AllergenId = Guid.NewGuid(),
                TenantId = tenantId,
                Name = request.Name,
                Description = request.Description,
                IsMajor = request.IsMajor,
                IsSystemDefined = false
            };

            await _allergenTagRepository.AddAsync(allergenTag);

            await _auditLogServices.LogAsync(
                tenantId,
                createdBy,
                "create",
                "allergen",
                allergenTag.AllergenId,
                null,
                JsonSerializer.Serialize(new
                {
                    allergenTag.Name,
                    allergenTag.Description,
                    allergenTag.IsMajor
                }));

            return allergenTag;
        }

        public async Task<AllergenTag> UpdateAsync(
            Guid allergenId, 
            UpdateAllergenTagRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var allergenTag = await _allergenTagRepository.GetByIdAndTenantAsync(allergenId, tenantId)
                ?? throw new KeyNotFoundException($"Allergen tag {allergenId} not found.");

            if (allergenTag.IsSystemDefined)
                throw new InvalidOperationException("System defined allergen tags cannot be modified.");

            var previousState = JsonSerializer.Serialize(new
            {
                allergenTag.Name,
                allergenTag.Description,
                allergenTag.IsMajor
            });

            if (request.Name != null) allergenTag.Name = request.Name;
            if (request.Description != null) allergenTag.Description = request.Description;
            if (request.IsMajor != null) allergenTag.IsMajor = request.IsMajor.Value;

            await _allergenTagRepository.UpdateAsync(allergenTag);

            var newState = JsonSerializer.Serialize(new
            {
                allergenTag.Name,
                allergenTag.Description,
                allergenTag.IsMajor
            });

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "update",
                "allergen",
                allergenTag.AllergenId,
                previousState,
                newState);

            return allergenTag;
        }

        public async Task DeleteAsync(Guid allergenId, Guid tenantId, Guid performedBy)
        {
            var allergenTag = await _allergenTagRepository.GetByIdAndTenantAsync(allergenId, tenantId)
                ?? throw new KeyNotFoundException($"Allergen tag {allergenId} not found.");

            if (allergenTag.IsSystemDefined)
                throw new InvalidOperationException("System defined allergen tags cannot be deleted.");

            var previousState = JsonSerializer.Serialize(new
            {
                allergenTag.Name,
                allergenTag.Description,
                allergenTag.IsMajor
            });

            await _allergenTagRepository.DeleteAsync(allergenId);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "delete",
                "allergen",
                allergenId,
                previousState,
                null);
        }
    }
}
