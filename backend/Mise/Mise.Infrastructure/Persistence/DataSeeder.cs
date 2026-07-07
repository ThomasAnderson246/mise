using Microsoft.EntityFrameworkCore;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MiseDbContext context)
        {
            Guid chefRoleId;

            

            // Only seed tenant, user and role if no tenants exist
            if (!await context.Tenants.AnyAsync())
            {
                // Create test tenant
                var tenant = new Tenant
                {
                    TenantId = Guid.NewGuid(),
                    Name = "Test Restaurant",
                    Slug = "test-restaurant",
                    DefaultUnitSystem = "imperial",
                    Tier = "pro",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Tenants.AddAsync(tenant);
                await context.SaveChangesAsync();

                // Create chef role
                var chefRole = new Role
                {
                    RoleId = Guid.NewGuid(),
                    TenantId = tenant.TenantId,
                    Name = "chef",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Roles.AddAsync(chefRole);
                await context.SaveChangesAsync();

                chefRoleId = chefRole.RoleId;

                // Create test user
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

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

                // Assign chef role to user
                var userRole = new UserRole
                {
                    UserRoleId = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = chefRoleId,
                    AssignedAt = DateTime.UtcNow
                };

                await context.UserRoles.AddAsync(userRole);
                await context.SaveChangesAsync();
            }
            else
            {
                // Tenant already exists — get the existing chef role
                var existingRole = await context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "chef");

                if (existingRole == null) return;

                chefRoleId = existingRole.RoleId;
            }

            if (!await context.AllergenTags.AnyAsync())
            {
                var tenant = await context.Tenants
                    .FirstOrDefaultAsync(t => t.Slug == "test-restaurant");

                if (tenant == null) return;

                var allergens = new List<AllergenTag>
                {
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Milk", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Eggs", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Fish", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Shell Fish", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Tree Nuts", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Peanuts", IsMajor = true, IsSystemDefined = true},
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Gluten", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Sesame", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = tenant.TenantId, Name = "Soy", IsMajor = true, IsSystemDefined = true }
                };

                await context.AllergenTags.AddRangeAsync(allergens);
                await context.SaveChangesAsync();
            }

            // Seed permissions only if none exist
            if (!await context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.create", Resource = "recipe", Action = "create" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.read", Resource = "recipe", Action = "read" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.update", Resource = "recipe", Action = "update" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.delete", Resource = "recipe", Action = "delete" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "recipe.publish", Resource = "recipe", Action = "publish" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "user.manage", Resource = "user", Action = "manage" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "audit.read", Resource = "audit", Action = "read" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.create", Resource = "menuitem", Action = "create" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.read", Resource = "menuitem", Action = "read" },
                    new Permission { PermissionId = Guid.NewGuid(), Name = "menuitem.update", Resource = "menuitem", Action = "update" },

                    //ingredient permissions
                    new Permission {PermissionId = Guid.NewGuid(), Name="ingredient.create", Resource="ingredient", Action="create" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="ingredient.read", Resource="ingredient", Action="read" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="ingredient.update", Resource="ingredient", Action="update" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="ingredient.delete", Resource="ingredient", Action="delete" },

                    new Permission {PermissionId = Guid.NewGuid(), Name="allergen.create", Resource="allergen", Action = "create" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="allergen.read", Resource="allergen", Action="read" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="allergen.update", Resource="allergen", Action="update" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="allergen.delete", Resource="allergen", Action="delete" },

                    new Permission {PermissionId = Guid.NewGuid(), Name="category.create", Resource="category", Action = "create" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="category.read", Resource="category", Action="read" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="category.update", Resource="category", Action="update" },
                    new Permission {PermissionId = Guid.NewGuid(), Name="category.delete", Resource="category", Action="delete" },

                };

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();

                // Assign all permissions to chef role
                var rolePermissions = permissions.Select(p => new RolePermission
                {
                    RoleId = chefRoleId,
                    PermissionId = p.PermissionId,
                    AssignedAt = DateTime.UtcNow
                }).ToList();

                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
