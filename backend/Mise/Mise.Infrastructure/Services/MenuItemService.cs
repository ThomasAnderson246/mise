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
using System.Transactions;

namespace Mise.Infrastructure.Services
{
    public class MenuItemService : IMenuItemService
    {

        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly MiseDbContext _context;

        public MenuItemService(IMenuItemRepository repository, IAuditLogServices auditLogServices, MiseDbContext context)
        {
            _menuItemRepository = repository;
            _auditLogServices = auditLogServices;
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync(Guid tenantId)
        {
            return await _menuItemRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<MenuItem?> GetByIdAsync(Guid menuItemId, Guid tenantId)
        {
            return await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId); 
        }

        public async Task<IEnumerable<MenuItem>> GetByCourseAsync(Guid tenantId, string course)
        {
            return await _menuItemRepository.GetByCourseAsync(tenantId, course);
        }

        public async Task<IEnumerable<MenuItem>> GetByStatusAsync(Guid tenantId, string status)
        {
            return await _menuItemRepository.GetByStatusAsync(tenantId, status);
        }

        public async Task<MenuItem> CreateAsync(
            CreateMenuItemRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            var nameExists = await _menuItemRepository.NameExistsInTenantAsync(tenantId, request.Name);
            if (nameExists)
                throw new InvalidOperationException($"A menu item with the name '{request.Name}' already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var menuItem = new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = request.Name,
                    Description = request.Description,
                    Course = request.Course,
                    Status = "draft",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _menuItemRepository.AddAsync(menuItem);

                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create",
                    "menuitem",
                    menuItem.MenuItemId,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        menuItem.Name,
                        menuItem.Course,
                        menuItem.Status
                    }));

                await transaction.CommitAsync();
                return menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MenuItem> UpdateAsync(
            Guid menuItemId,
            UpdateMenuItemRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} not found.");

            if (request.Name != null)
            {
                var nameExists = await _menuItemRepository.NameExistsInTenantAsync(tenantId, request.Name);
                if (nameExists && request.Name.ToLower() != menuItem.Name.ToLower())
                    throw new InvalidOperationException($"A menu item with the name '{request.Name}' already exists.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new
                {
                    menuItem.Name,
                    menuItem.Description,
                    menuItem.Course
                });

                if (request.Name != null) menuItem.Name = request.Name;
                if (request.Description != null) menuItem.Description = request.Description;
                if (request.Course != null) menuItem.Course = request.Course;
                menuItem.UpdatedAt = DateTime.UtcNow;

                await _menuItemRepository.UpdateAsync(menuItem);

                var newState = JsonSerializer.Serialize(new
                {
                    menuItem.Name,
                    menuItem.Description,
                    menuItem.Course
                });

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "update",
                    "menuitem",
                    menuItem.MenuItemId,
                    previousState,
                    newState);

                await transaction.CommitAsync();
                return menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid menuItemId, Guid tenantId, Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} not found.");

            if (menuItem.Status == "published")
                throw new InvalidOperationException("Published menu items cannot be deleted.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new
                {
                    menuItem.Name,
                    menuItem.Course,
                    menuItem.Status
                });

                await _menuItemRepository.DeleteAsync(menuItemId);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "delete",
                    "menuitem",
                    menuItemId,
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

        public async Task<MenuItem> PublishAsync(
            Guid menuItemId,
            Guid tenantId,
            Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} not found.");

            if (menuItem.Status == "published")
                throw new InvalidOperationException("Menu item is already published.");

            if (!menuItem.MenuItemRecipes.Any())
                throw new InvalidOperationException("Cannot publish a menu item with no linked recipes.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                menuItem.Status = "published";
                menuItem.UpdatedAt = DateTime.UtcNow;

                await _menuItemRepository.UpdateAsync(menuItem);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "publish",
                    "menuitem",
                    menuItemId,
                    null,
                    JsonSerializer.Serialize(new { menuItem.Name, menuItem.Status }));

                await transaction.CommitAsync();
                return menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MenuItem> AddRecipeAsync(
            Guid menuItemId,
            AddMenuItemRecipeRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} does not exist.");
            var recipeExists = await _context.Recipes
                .AnyAsync(r => r.RecipeId == request.RecipeId && r.TenantId == tenantId);
            if (!recipeExists)
                throw new KeyNotFoundException($"Recipe {request.RecipeId} not found.");

            var alreadyLinked = await _context.MenuItemRecipes
                .AnyAsync(mir => mir.MenuItemId == menuItemId && mir.RecipeId == request.RecipeId);
            if (alreadyLinked)
                throw new InvalidOperationException("This recipe is already linked to the menu item.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var menuItemRecipe = new MenuItemRecipe
                {
                    MenuItemRecipeId = Guid.NewGuid(),
                    MenuItemId = menuItemId,
                    RecipeId = request.RecipeId,
                    DisplayOrder = request.DisplayOrder,
                    Note = request.Note
                };

                await _context.MenuItemRecipes.AddAsync(menuItemRecipe);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "add_recipe",
                    "menuitem",
                    menuItemId,
                    null,
                    JsonSerializer.Serialize(new { RecipeId = request.RecipeId }));

                await transaction.CommitAsync();

                return await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                    ?? menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<MenuItem> RemoveRecipeAsync(
            Guid menuItemId,
            Guid recipeId,
            Guid tenantId,
            Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} not found.");

            var menuItemRecipe = await _context.MenuItemRecipes
                .FirstOrDefaultAsync(mir => mir.MenuItemId == menuItemId && mir.RecipeId == recipeId)
                ?? throw new KeyNotFoundException("Recipe is not linked to this menu item");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.MenuItemRecipes.Remove(menuItemRecipe);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "remove_recipe",
                    "menuitem",
                    menuItemId,
                    JsonSerializer.Serialize(new { RecipeId = recipeId }),
                    null);

                await transaction.CommitAsync();

                return await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                    ?? menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<MenuItem> ResolveAllergensAsync(
            Guid menuItemId,
            Guid tenantId,
            Guid performedBy)
        {
            var menuItem = await _menuItemRepository.GetWithFullDetailsAsync(menuItemId, tenantId)
                ?? throw new KeyNotFoundException($"Menu item {menuItemId} not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingAuto = await _context.MenuItemAllergens
                    .Where(mia => mia.MenuItemId == menuItemId && !mia.IsManual)
                    .ToListAsync();

                _context.MenuItemAllergens.RemoveRange(existingAuto);
                await _context.SaveChangesAsync();

                var resolvedAllergens = new List<MenuItemAllergen>();

                foreach (var menuItemRecipe in menuItem.MenuItemRecipes)
                {
                    var recipe = menuItemRecipe.Recipe;
                    if (recipe?.CurrentVersion?.Ingredients == null) continue;

                    foreach(var recipeIngredient in recipe.CurrentVersion.Ingredients)
                    {
                        var ingredient = recipeIngredient.Ingredient;
                        if (ingredient?.IngredientAllergens == null) continue;

                        foreach (var ingredientAllergen in ingredient.IngredientAllergens)
                        {
                            var alreadyResolved = resolvedAllergens
                                .Any(ra => ra.AllergenId == ingredientAllergen.AllergenId);

                            if (!alreadyResolved)
                            {
                                resolvedAllergens.Add(new MenuItemAllergen
                                {
                                    MenuItemAllergenId = Guid.NewGuid(),
                                    MenuItemId = menuItemId,
                                    AllergenId = ingredientAllergen.AllergenId,
                                    SourceName = ingredient.Name,
                                    SourceRecipeId = recipe.RecipeId,
                                    SourceComponent = recipe.Title,
                                    IsDirect = true,
                                    IsManual = false,
                                    CreatedAt = DateTime.Now,
                                });
                            }
                        }
                    }
            
                }
                if (resolvedAllergens.Any())
                {
                    await _context.MenuItemAllergens.AddRangeAsync(resolvedAllergens);
                    await _context.SaveChangesAsync();
                }

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "resolve_allergens",
                    "menuitem",
                    menuItemId,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        ResolvedCount = resolvedAllergens.Count,
                    }));

                await transaction.CommitAsync();

                return await _menuItemRepository.GetByIdAndTenantAsync(menuItemId, tenantId)
                    ?? menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
