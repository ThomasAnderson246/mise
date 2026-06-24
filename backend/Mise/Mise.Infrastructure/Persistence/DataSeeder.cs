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
