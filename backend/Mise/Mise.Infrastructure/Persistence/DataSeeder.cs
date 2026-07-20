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
                var newTenant = new Tenant
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

                await context.Tenants.AddAsync(newTenant);
                await context.SaveChangesAsync();

                // Create chef role
                var chefRole = new Role
                {
                    RoleId = Guid.NewGuid(),
                    TenantId = newTenant.TenantId,
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
                    TenantId = newTenant.TenantId,
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

            if (!await context.UnitTypes.AnyAsync())
            {
                var unitTenant = await context.Tenants
                    .FirstOrDefaultAsync(t => t.Slug == "test-restaurant");

                if (unitTenant != null)
                {
                    var unitTypes = new List<UnitType>
                        {
                            // weight
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Gram", Abbreviation = "g", System = "metric", MeasureType = "weight", ConversionFactor = 1m, IsSystemDefined = true},
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Kilogram", Abbreviation = "kg", System = "metric", MeasureType = "weight", ConversionFactor = 1000m, IsSystemDefined = true},
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Ounce", Abbreviation = "oz", System = "imperial", MeasureType = "weight", ConversionFactor = 28.3495m, IsSystemDefined= true},
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Pound", Abbreviation = "lb", System = "imperial", MeasureType = "weight", ConversionFactor = 453.592m, IsSystemDefined = true },

                            // volume
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Milliliter", Abbreviation = "ml", System = "metric", MeasureType = "volume", ConversionFactor = 1m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Liter", Abbreviation = "l", System = "metric", MeasureType = "volume", ConversionFactor = 1000m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Teaspoon", Abbreviation = "tsp", System = "imperial", MeasureType = "volume", ConversionFactor = 4.92892m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Tablespoon", Abbreviation = "tbsp", System = "imperial", MeasureType = "volume", ConversionFactor = 14.7868m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Fluid Ounce", Abbreviation = "fl oz", System = "imperial", MeasureType = "volume", ConversionFactor = 29.573m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Cup", Abbreviation = "c", System="imperial", MeasureType="volume", ConversionFactor=236.588m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Pint", Abbreviation = "pt", System = "imperial", MeasureType = "volume", ConversionFactor = 473.176m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Quart", Abbreviation = "qt", System = "imperial", MeasureType="volume", ConversionFactor = 946.353m, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Gallon", Abbreviation = "gal", System = "imperial", MeasureType = "volume", ConversionFactor = 3785.41m, IsSystemDefined = true },

                            //count - universal
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Each", Abbreviation = "ea", System = "universal", MeasureType = "count", IsNonConvertible = true, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Dozen", Abbreviation = "dz", System = "universal", MeasureType = "count", IsNonConvertible = true, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Pinch", Abbreviation = "pinch", System = "universal", MeasureType = "count", IsNonConvertible = true, IsSystemDefined = true },
                            new UnitType {UnitTypeId = Guid.NewGuid(), TenantId = unitTenant.TenantId, Name = "Bunch", Abbreviation = "bunch", System = "universal", MeasureType = "count", IsNonConvertible = true, IsSystemDefined = true },
                        };

                    await context.UnitTypes.AddRangeAsync(unitTypes);
                    await context.SaveChangesAsync();
                }
            }

            if (!await context.AllergenTags.AnyAsync())
            {
                var allergenTenent = await context.Tenants
                    .FirstOrDefaultAsync(t => t.Slug == "test-restaurant");

                if (allergenTenent == null) return;

                var allergens = new List<AllergenTag>
                {
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Milk", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Eggs", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Fish", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Shell Fish", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Tree Nuts", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Peanuts", IsMajor = true, IsSystemDefined = true},
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Gluten", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Sesame", IsMajor = true, IsSystemDefined = true },
                    new AllergenTag { AllergenId = Guid.NewGuid(), TenantId = allergenTenent.TenantId, Name = "Soy", IsMajor = true, IsSystemDefined = true }
                };

                await context.AllergenTags.AddRangeAsync(allergens);
                await context.SaveChangesAsync();
            }

            var tenant = await context.Tenants
                .FirstOrDefaultAsync(t => t.Slug == "test-restaurant");

            if (tenant != null)
            {
                var existingRoleNames = await context.Roles
                    .Where(r => r.TenantId == tenant.TenantId)
                    .Select(r => r.Name)
                    .ToListAsync();

                var predefinedroles = new List<string>
                {
                    "owner",
                    "head chef",
                    "sous chef",
                    "cook",
                    "foh manager",
                    "foh staff"
                };

                var newRoles = predefinedroles
                    .Where(name => !existingRoleNames.Contains(name))
                    .Select(name => new Role
                    {
                        RoleId = Guid.NewGuid(),
                        TenantId = tenant.TenantId,
                        Name = name,
                        IsSystemRole = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    }).ToList();

                if (newRoles.Any())
                {
                    await context.Roles.AddRangeAsync(newRoles);
                    await context.SaveChangesAsync();
                }
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
                ("unit.create", "unit", "create"),
                ("unit.read", "unit", "read"),
                ("unit.update", "unit", "update"),
                ("unit.delete", "unit", "delete"),
                ("preplist.create", "preplist", "create"),
                ("preplist.read", "preplist", "read"),
                ("preplist.update", "preplist", "update"),
                ("preplist.delete", "preplist", "delete"),
                ("preplist.complete", "preplist", "complete"),
                ("preplist.manage", "preplist", "manage"),
                ("notification.read", "notification", "read"),
                ("notification.send", "notification", "send"),
                ("notification.broadcast", "notification", "broadcast")
                

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
