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

            // add new permissions as we add them
            var allPermissions = new List<(string Name, string Resource, string Action)>
            {
                ("recipe.create", "recipe", "create"),
                ("recipe.read", "recipe", "read"),
                ("recipe.update", "recipe", "update"),
                ("recipe.delete", "recipe", "delete"),
                ("recipe.publish", "recipe", "publish"),
                ("user.manage", "user", "manage"),
                ("audit.read", "audit", "read"),
                ("menuitem.create", "menuitem", "create"),
                ("menuitem.read", "menuitem", "read"),
                ("menuitem.update", "menuitem", "update"),
                ("menuitem.delete", "menuitem", "delete"),
                ("ingredient.create", "ingredient", "create"),
                ("ingredient.read", "ingredient", "read"),
                ("ingredient.update", "ingredient", "update"),
                ("ingredient.delete", "ingredient", "delete"),
                ("allergen.create", "allergen", "create"),
                ("allergen.read", "allergen", "read"),
                ("allergen.update", "allergen", "update"),
                ("allergen.delete", "allergen", "delete"),
                ("category.create", "category", "create"),
                ("category.read", "category", "read"),
                ("category.update", "category", "update"),
                ("category.delete", "category", "delete"),
                

            };

            var existingPermissionNames = await context.Permissions
                .Select(p => p.Name)
                .ToListAsync();

            var newPermissions = allPermissions
                .Where(p => !existingPermissionNames.Contains(p.Name))
                .Select(p => new Permission
                {
                    PermissionId = Guid.NewGuid(),
                    Name = p.Name,
                    Resource = p.Resource,
                    Action = p.Action,
                }).ToList();

            if (newPermissions.Any())
            {
                await context.Permissions.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();
            }
            var existingRolePermissionIds = await context.RolePermissions
                .Where(rp => rp.RoleId == chefRoleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var allPermissionEntities = await context.Permissions.ToListAsync();

            var newRolePermissions = allPermissionEntities
                .Where(p => !existingRolePermissionIds.Contains(p.PermissionId))
                .Select(p => new RolePermission
                {
                    RoleId = chefRoleId,
                    PermissionId = p.PermissionId,
                    AssignedAt = DateTime.UtcNow
                }).ToList();

            if (newRolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(newRolePermissions);
                await context.SaveChangesAsync();
            }
            
        }
    }
}
