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
using Microsoft.EntityFrameworkCore;

namespace Mise.Infrastructure.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IAuditLogServices _auditLogServices;
        private readonly MiseDbContext _context;

        public UserService(IUserRepository userRepository, IAuthService authService, IAuditLogServices auditLogServices, MiseDbContext context)
        {
            _userRepository = userRepository;
            _authService = authService;
            _auditLogServices = auditLogServices;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync(Guid tenantId)
        {
            return await _userRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<User?> GetByIdAsync(Guid userId, Guid tenantId)
        {
            return await _userRepository.GetByIdAndTenantAsync(userId, tenantId);
        }

        public async Task<(User user, string temporaryPassword)> InviteAsync(
            InviteUserRequest request,
            Guid tenantId,
            Guid invitedBy)
        {
            var emailExists = await _userRepository.EmailExistsInTenantAsync(request.Email, tenantId);
            if (emailExists)
                throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");

            var tempPassword = GenerateTemporaryPassword();
            var passwordHash = _authService.HashPassword(tempPassword);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenantId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                Status = "pending",
                UnitPreference = request.UnitPreference,
                MustChangePassword = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var userRoles = request.RoleIds.Select(rId => new UserRole
                {
                    UserRoleId = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = rId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = invitedBy
                }).ToList();

                await _context.UserRoles.AddRangeAsync(userRoles);
                await _context.SaveChangesAsync();
            }

            await _auditLogServices.LogAsync(
                tenantId,
                invitedBy,
                "invite",
                "user",
                user.UserId,
                null,
                JsonSerializer.Serialize(new
                {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Status
                }));

            return (user, tempPassword);
        }

        public async Task<User> UpdateAsync(
            Guid userId,
            UpdateUserRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var user = await _userRepository.GetByIdAndTenantAsync(userId, tenantId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            var previousState = JsonSerializer.Serialize(new
            {
                user.FirstName,
                user.LastName,
                user.UnitPreference
            });

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if(request.LastName != null) user.LastName = request.LastName;
            if (request.UnitPreference != null) user.UnitPreference = request.UnitPreference;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var newState = JsonSerializer.Serialize(new
            {
                user.FirstName,
                user.LastName,
                user.UnitPreference
            });

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "update",
                "user",
                user.UserId,
                previousState,
                newState);

            return user;
        }

        public async Task DeactivateAsync(Guid userId, Guid tenantId, Guid performedBy)
        {
            var user = await _userRepository.GetByIdAndTenantAsync(userId, tenantId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            if (user.Status == "inactive")
                throw new InvalidOperationException("User is already inactive.");

            user.Status = "inactive";
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "deactivate",
                "user",
                user.UserId,
                "active",
                "inactive");
        }

        public async Task ReactivateAsync(Guid userId, Guid tenantId, Guid performedBy)
        {
            var user = await _userRepository.GetByIdAndTenantAsync(userId, tenantId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            if (user.Status == "active")
                throw new InvalidOperationException("User is already active.");

            user.Status = "active";
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "reactivate",
                "user",
                user.UserId,
                "inactive",
                "active");
        }

        public async Task AssignRoleAsync(
            Guid userId,
            Guid roleId,
            Guid tenantId,
            Guid assignedBy)
        {
            var user = await _userRepository.GetByIdAndTenantAsync(userId, tenantId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId && r.TenantId == tenantId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            var alreadyAssigned = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (alreadyAssigned)
                throw new InvalidOperationException("User already has this role.");

            var userRole = new UserRole
            {
                UserRoleId = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            await _auditLogServices.LogAsync(
                tenantId,
                assignedBy,
                "assign_role",
                "user",
                userId,
                null,
                JsonSerializer.Serialize(new { RoleName = role.Name }));
        }

        public async Task RemoveRoleAsync(
            Guid userId,
            Guid roleId,
            Guid tenantId,
            Guid performedBy)
        {

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId)
                ?? throw new KeyNotFoundException("User does not have this role.");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FindAsync(roleId);

            await _auditLogServices.LogAsync(
                tenantId,
                performedBy,
                "remove_role",
                "user",
                userId,
                JsonSerializer.Serialize(new { roleName = role?.Name }),
                null);
        }


        private static string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
            var random = new Random();
            return new string(Enumerable.Range(0, 12)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
