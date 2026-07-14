using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class UnitTypeService : IUnitTypeService
    {
        private readonly IUnitTypeRepository _unitTypeRepository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly MiseDbContext _context;

        public UnitTypeService(IUnitTypeRepository unitTypeRepository, IAuditLogServices auditLogServices, MiseDbContext context)
        {
            _unitTypeRepository = unitTypeRepository;
            _auditLogServices = auditLogServices;
            _context = context;
        }

        public async Task<IEnumerable<UnitType>> GetAllAsync(Guid tenantId)
        {
            return await _unitTypeRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<UnitType?> GetByIdAsync(Guid unittypeId, Guid tenantId)
        {
            return await _unitTypeRepository.GetByIdAndTenantAsync(unittypeId, tenantId);
        }

        public async Task<IEnumerable<UnitType>> GetByMeasureTypeAsync(Guid tenantId, string measureType)
        {
            return await _unitTypeRepository.GetByMeasureTypeAsync(tenantId, measureType);
        }

        public async Task<IEnumerable<UnitType>> GetBySystemAsync(Guid tenantId, string system)
        {
            return await _unitTypeRepository.GetBySystemAsync(tenantId, system);
        }

        public async Task<UnitType> CreateAsync(
            CreateUnitTypeRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            var nameExists = await _unitTypeRepository.NameExistsInTenantAsync(tenantId, request.Name);
            if (nameExists)
                throw new InvalidOperationException($"A unit type with the name '{request.Name} already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var unitType = new UnitType
                {
                    UnitTypeId = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = request.Name,
                    Abbreviation = request.Abbreviation,
                    System = request.System,
                    MeasureType = request.MeasureType,
                    ConversionFactor = request.ConversionFactor,
                    IsNonConvertible = request.IsNonConvertible,
                    IsSystemDefined = false
                };

                await _unitTypeRepository.AddAsync(unitType);

                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create",
                    "unit_type",
                    unitType.UnitTypeId,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        unitType.Name,
                        unitType.Abbreviation,
                        unitType.System,
                        unitType.MeasureType
                    }));

                await transaction.CommitAsync();
                return unitType;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<UnitType> UpdateAsync(
            Guid unitTypeId,
            UpdateUnityTypeRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var unitType = await _unitTypeRepository.GetByIdAndTenantAsync(unitTypeId, tenantId)
                ?? throw new KeyNotFoundException($"Unit type {unitTypeId} not found.");

            if (unitType.IsSystemDefined)
                throw new InvalidOperationException("System-defined unit types cannot be changed.");

            if (request.Name != null)
            {
                var nameExists = await _unitTypeRepository.NameExistsInTenantAsync(tenantId, request.Name);
                if (nameExists && request.Name.ToLower() != unitType.Name.ToLower())
                    throw new InvalidOperationException($"A unit type with the name {request.Name} already exists.");
            }

            using var transaciton = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new
                {
                    unitType.Name,
                    unitType.Abbreviation,
                    unitType.ConversionFactor,
                    unitType.IsNonConvertible
                });

                if (request.Name != null) unitType.Name = request.Name;
                if (request.Abbreviation != null) unitType.Abbreviation = request.Abbreviation;
                if (request.ConversionFactor != null) unitType.ConversionFactor = request.ConversionFactor;
                if (request.IsNonConvertible != null) unitType.IsNonConvertible = request.IsNonConvertible.Value;

                await _unitTypeRepository.UpdateAsync(unitType);

                var newState = JsonSerializer.Serialize(new
                {
                    unitType.Name,
                    unitType.Abbreviation,
                    unitType.ConversionFactor,
                    unitType.IsNonConvertible
                });

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "update",
                    "unit_type",
                    unitType.UnitTypeId,
                    previousState,
                    newState);

                await transaciton.CommitAsync();
                return unitType;
            }
            catch
            {
                await transaciton.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid unitTypeId, Guid tenantId, Guid performedBy)
        {
            var unitType = await _unitTypeRepository.GetByIdAndTenantAsync(unitTypeId, tenantId)
                ?? throw new KeyNotFoundException($"Unit type {unitTypeId} not found.");

            if (unitType.IsSystemDefined)
                throw new InvalidOperationException("System-defined unit types cannot be deleted.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new
                {
                    unitType.Name,
                    unitType.Abbreviation,
                    unitType.System,
                    unitType.MeasureType
                });

                await _unitTypeRepository.DeleteAsync(unitTypeId);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "delete",
                    "unit_type",
                    unitTypeId,
                    previousState,
                    null);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
