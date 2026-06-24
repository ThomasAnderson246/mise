using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MiseDbContext context)
        {
            //only seed if no tenants exist
            if (await context.Tenants.AnyAsync()) return;

            // creates tenant
            var tenant = new Tenant
            {
                TenantId = Guid.NewGuid(),
                Name = "Test Restaurant",
                Slug = "test-restaurant",
                DefaultUnitSystem = "imperial",
                Tier = "pro",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync();

            // create chef role
            var chefRole = new Role
            {
                RoleId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                Name = "chef",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await context.Roles.AddAsync(chefRole);
            await context.SaveChangesAsync();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                Email = "chef@testrestaurant.com",
                PasswordHash = passwordHash,
                FirstName = "Test",
                LastName = "Chef",
                Status = "active",
                UnitPreference = "imperial",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var permissions = new List<Permission>
            {
                // recipe permissions
                new Permission {PermissionId = Guid.NewGuid(), Name= "recipe.create", Resource = "recipe", Action = "create"},
                new Permission { PermissionId = Guid.NewGuid(), Name="recipe.read", Resource="recipe", Action="create" },
                new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.update", Resource = "recipe", Action="update" },
                new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.delete", Resource = "recipe", Action = "delete" },
                new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.publish", Resource = "recipe", Action = "publish" },

                // user permissions

                new Permission { PermissionId = Guid.NewGuid(), Name = "user.manage", Resource = "user", Action = "manage" },

                //audit permissions
                new Permission { PermissionId = Guid.NewGuid(), Name = "audit.read", Resource = "audit", Action = "read" },

                //menu item permissions
                new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.create", Resource = "menuitem", Action = "create" },
                new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.read", Resource = "menuitem", Action = "read" },
                new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.update", Resource = "menuitem", Action = "update" },
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();

            var rolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = chefRole.RoleId,
                PermissionId = p.PermissionId,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();

            //assign new role to new chef
            var userRole = new UserRole

            {
                UserRoleId = Guid.NewGuid(),
                UserId = user.UserId,
                RoleId = chefRole.RoleId,
                AssignedAt = DateTime.UtcNow,
            };

            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }
    }
}
