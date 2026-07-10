using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;
using System.Security.Cryptography.X509Certificates;

namespace Mise.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly MiseDbContext _context;

        public RoleService(IRoleRepository roleRepository, IAuditLogServices auditLogServices, MiseDbContext context)
        {
            _roleRepository = roleRepository;
            _auditLogServices = auditLogServices;
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync(Guid tenantId)
        {
            return await _roleRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<Role?> GetByIdAsync(Guid roleId, Guid tenantId)
        {
            return await _roleRepository.GetByIdAndTenantAsync(roleId, tenantId);
        }

        public async Task<Role> CreateAsync(
            CreateRoleRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            var nameExists = await _roleRepository.NameExistsInTenantAsync(tenantId, request.Name);
            if (nameExists)
                throw new InvalidOperationException($"A role with the name '{request.Name}' already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var role = new Role
                {
                    RoleId = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = request.Name,
                    IsSystemRole = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _roleRepository.AddAsync(role);

                if (request.PermissionIds != null && request.PermissionIds.Any())
                {
                    var rolePermissions = request.PermissionIds.Select(pId => new RolePermission
                    {
                        RoleId = role.RoleId,
                        PermissionId = pId,
                        AssignedAt = DateTime.UtcNow
                    }).ToList();

                    await _context.RolePermissions.AddRangeAsync(rolePermissions);
                    await _context.SaveChangesAsync();
                }

                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create",
                    "role",
                    role.RoleId,
                    null,
                    JsonSerializer.Serialize(new { role.Name }));

                await transaction.CommitAsync();
                return role;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Role> UpdateAsync(
            Guid roleId,
            UpdateRoleRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var role = await _roleRepository.GetByIdAndTenantAsync(roleId, tenantId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            if (role.IsSystemRole)
                throw new InvalidOperationException("System roles cannot be renamed.");

            if(request.Name != null)
            {
                var nameExists = await _roleRepository.NameExistsInTenantAsync(tenantId, request.Name);
                if (nameExists && request.Name.ToLower() != role.Name.ToLower())
                    throw new InvalidOperationException($"A role with the name '{request.Name}' already exists.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new { role.Name });

                if (request.Name != null) role.Name = request.Name;
                role.UpdatedAt = DateTime.UtcNow;

                await _roleRepository.UpdateAsync(role);

                var newState = JsonSerializer.Serialize(new { role.Name });

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "update",
                    "role",
                    role.RoleId,
                    previousState,
                    newState);

                await transaction.CommitAsync();
                return role;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

           
        }

        public async Task DeleteAsync(Guid roleId, Guid tenantId, Guid performedBy)
        {
            var role = await _roleRepository.GetByIdAndTenantAsync(roleId, tenantId)
                ?? throw new KeyNotFoundException($"role {roleId} not found.");

            if (role.IsSystemRole)
                throw new InvalidOperationException("System roles cannot be deleted.");

            var hasUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
            if (hasUsers)
                throw new InvalidOperationException("Cannot delete a role that is assigned to a user.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new { role.Name });

                await _roleRepository.DeleteAsync(roleId);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "delete",
                    "role",
                    roleId,
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

        public async Task AssignPermissionAsync(
            Guid roleId,
            Guid permissionsId,
            Guid tenantId,
            Guid performedBy)
        {
            var role = await _roleRepository.GetByIdAndTenantAsync(roleId, tenantId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.PermissionId == permissionsId)
                ?? throw new KeyNotFoundException($"Permissions {permissionsId} not found.");

            var alreadyAssigned = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionsId);

            if (alreadyAssigned)
                throw new InvalidOperationException("Role already has this permissions.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionsId,
                    AssignedAt = DateTime.UtcNow
                };

                await _context.RolePermissions.AddAsync(rolePermission);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "assign_permission",
                    "role",
                    roleId,
                    null,
                    JsonSerializer.Serialize(new { PermissionName = permission.Name }));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemovePermissionAsync(
            Guid roleId,
            Guid permissionId,
            Guid tenantId,
            Guid performedBy)
        {
            var role = await _roleRepository.GetByIdAndTenantAsync(roleId, tenantId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
                ?? throw new KeyNotFoundException("Role does not have this permission.");

            var permission = await _context.Permissions.FindAsync(permissionId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "remove_permission",
                    "role",
                    roleId,
                    JsonSerializer.Serialize(new { PermissionName = permission?.Name }),
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
