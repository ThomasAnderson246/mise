using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Persistence.Context
{
	public class MiseDbContext : DbContext
	{

        public MiseDbContext(DbContextOptions<MiseDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }

        public DbSet<AllergenTag> AllergenTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<IngredientAllergen> IngredientAllergens { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeVersion> RecipeVersions { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<SubRecipe> SubRecipes { get; set; }
        public DbSet<CookingSession> CookingSessions { get; set; }
        public DbSet<StepCheckOff> StepCheckOffs { get; set; }
        public DbSet<IngredientCheckOff> IngredientCheckOffs { get; set; }
        public DbSet<PrepList> PrepLists { get; set; }
        public DbSet<PrepListItem> PrepListItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationQueue> NotificationsQueues { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<TranscriptionJob> TranscriptionJobs { get; set; }
        public DbSet<TranscriptionResult> TranscriptionResults { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemRecipe> MenuItemRecipes { get; set; }
        public DbSet<MenuItemAllergen> MenuItemAllergens { get; set; }
        public DbSet<RecipeIngredientGroup> RecipeIngredientGroups { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiseDbContext).Assembly);
        }
    }
}
