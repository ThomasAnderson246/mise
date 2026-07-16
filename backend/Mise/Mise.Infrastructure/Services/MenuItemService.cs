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

                await transaction.CommitAsync(0);
                return menuItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MenuItem> AddRecipeAsync() { }
    }
}
