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

namespace Mise.Infrastructure.Services
{
    public class PrepListService : IPrepListService
    {
        private readonly IPrepListRepository _prepListRepository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly MiseDbContext _context;

        public PrepListService(IPrepListRepository prepListRepository, IAuditLogServices auditLogServices, MiseDbContext context)
        {
            _prepListRepository = prepListRepository;
            _auditLogServices = auditLogServices;
            _context = context;
        }

        public async Task<IEnumerable<PrepList>> GetAllAsync(Guid tenantId)
        {
            return await _prepListRepository.GetAllByTenantAsync(tenantId);
        }

        public async Task<PrepList?> GetByIdAsync(Guid prepListId, Guid tenantId)
        {
            return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId);
        }

        public async Task<IEnumerable<PrepList>> GetByStatusAsync(Guid tenantId, bool isComplete)
        {
            return await _prepListRepository.GetByStatusAsync(tenantId, isComplete);
        }

        public async Task<PrepList> CreateAsync(
            CreatePrepListRequest request,
            Guid tenantId,
            Guid createdBy)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var prepList = new PrepList
                {
                    PrepListId = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = request.Name,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow,
                    IsComplete = false
                };

                await _prepListRepository.AddAsync(prepList);

                await _auditLogServices.LogAsync(
                    tenantId,
                    createdBy,
                    "create",
                    "prep_list",
                    prepList.PrepListId,
                    null,
                    JsonSerializer.Serialize(new { prepList.Name }));

                await transaction.CommitAsync();
                return prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid prepListId, Guid tenantId, Guid performedBy)
        {
            var prepList = await _prepListRepository.GetByIdAndTenantAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            if (prepList.IsComplete)
                throw new InvalidOperationException("Completed prep lists cannot be deleted.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previosState = JsonSerializer.Serialize(new { prepList.Name });

                await _prepListRepository.DeleteAsync(prepListId);

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "delete",
                    "prep_list",
                    prepListId,
                    previosState,
                    null);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrepList> AddItemAsync(
            Guid prepListId,
            AddPrepListItemRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            if (prepList.IsComplete)
                throw new InvalidOperationException("Cannot add items to a completed prep list.");

            var recipeExists = await _context.Recipes
                .AnyAsync(r => r.RecipeId == request.RecipeId && r.TenantId == tenantId);
            if (!recipeExists)
                throw new KeyNotFoundException($"Recipe {request.RecipeId} not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var item = new PrepListItem
                {
                    PrepListItemId = Guid.NewGuid(),
                    PrepListId = prepListId,
                    RecipeId = request.RecipeId,
                    DisplayOrder = request.DisplayOrder,
                    ScalingFactor = request.ScalingFactor,
                    IsComplete = false
                };

                await _context.PrepListItems.AddAsync(item);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "add_item",
                    "prep_list",
                    prepListId,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        RecipeId = request.RecipeId,
                        request.ScalingFactor,
                        request.DisplayOrder
                    }));

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrepList> UpdateItemAsync(
            Guid prepListId,
            Guid itemId,
            UpdatePrepListItemRequest request,
            Guid tenantId,
            Guid performedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            var item = prepList.Items.FirstOrDefault(i => i.PrepListItemId == itemId)
                ?? throw new KeyNotFoundException($"Prep list item {itemId} not found.");

            if ( (item.IsComplete))
            
            throw new InvalidOperationException("Cnanot update a completed prep list item.");

            using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var previousState = JsonSerializer.Serialize(new
                    {
                        item.DisplayOrder,
                        item.ScalingFactor
                    });

                    if (request.DisplayOrder != null) item.DisplayOrder = request.DisplayOrder.Value;
                    if (request.ScalingFactor != null) item.DisplayOrder = request.DisplayOrder.Value;

                    _context.PrepListItems.Update(item);
                    await _context.SaveChangesAsync();

                    var newState = JsonSerializer.Serialize(new
                    {
                        item.DisplayOrder,
                        item.ScalingFactor
                    });

                    await _auditLogServices.LogAsync(
                        tenantId,
                        performedBy,
                        "update_item",
                        "prep_list",
                        prepListId,
                        previousState,
                        newState);

                    await transaction.CommitAsync();

                    return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                        ?? prepList;
                }
                catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        public async Task<PrepList> RemoveItemAsync(
            Guid prepListId,
            Guid itemId,
            Guid tenantId,
            Guid performedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            if (prepList.IsComplete)
                throw new InvalidOperationException("Cannot remove items from a completed list.");

            var item = prepList.Items.FirstOrDefault(i => i.PrepListItemId == itemId)
                ?? throw new KeyNotFoundException($"Prep list item {itemId} not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var previousState = JsonSerializer.Serialize(new
                {
                    item.RecipeId,
                    item.ScalingFactor,
                    item.DisplayOrder
                });

                _context.PrepListItems.Remove(item);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    performedBy,
                    "remove_item",
                    "prep_list",
                    prepListId,
                    previousState,
                    null);

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrepList> CompleteItemAsync(
            Guid prepListId,
            Guid itemId,
            Guid tenantId,
            Guid completedBy
            )
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            

            var item = prepList.Items.FirstOrDefault(i => i.PrepListItemId == itemId)
                ?? throw new KeyNotFoundException($"Prep list item {itemId} not found.");

            if (item.IsComplete)
                throw new InvalidOperationException("Item is already complete.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                item.IsComplete = true;
                item.CompletedBy = completedBy;
                item.CompletedAt = DateTime.UtcNow;

                _context.PrepListItems.Update(item);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    completedBy,
                    "complete_item",
                    "prepList",
                    prepListId,
                    null,
                    JsonSerializer.Serialize(new { ItemId = itemId }));

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrepList> CompletePrepListAsync(
            Guid prepListId,
            Guid tenantId,
            Guid completedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            

            if (prepList.IsComplete)
                throw new InvalidOperationException("Prep list is already complete.");

            var hasIncompleteItems = prepList.Items.Any(i => !i.IsComplete);
            if (hasIncompleteItems)
                throw new InvalidOperationException("All items must be completed before the prep list can be completed.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                prepList.IsComplete = true;
                prepList.CompletedAt = DateTime.UtcNow;

                await _prepListRepository.UpdateAsync(prepList);

                await _auditLogServices.LogAsync(
                    tenantId,
                    completedBy,
                    "complete",
                    "prep_list",
                    prepListId,
                    null,
                    JsonSerializer.Serialize(new { CompletedAt = prepList.CompletedAt }));

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PrepListSummaryResponse>> GetSummaryAsync(Guid tenantId)
        {
            var prepLists = await _context.PrepLists
                .Where(pl => pl.TenantId == tenantId && !pl.IsComplete)
                .Include(pl => pl.Items)
                .Include(pl => pl.CreatedByUser)
                .OrderBy(pl => pl.CreatedAt)
                .ToListAsync();

            return prepLists.Select(pl => new PrepListSummaryResponse
            {
                PrepListId = pl.PrepListId,
                Name = pl.Name,
                CreatedBy = pl.CreatedBy,
                CreatedByName = pl.CreatedByUser != null
                    ? $"{pl.CreatedByUser.FirstName} {pl.CreatedByUser.LastName}"
                    : null,
                TotalItems = pl.Items.Count,
                CompletedItems = pl.Items.Count(i => i.IsComplete),
                IsComplete = pl.IsComplete,
                CreatedAt = pl.CreatedAt
            });
        }

        public async Task<PrepList> ForceCompleteItemAsync(
            Guid prepListId,
            Guid itemId,
            Guid tenantId,
            Guid completedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            var item = prepList.Items.FirstOrDefault(i => i.PrepListItemId == itemId)
                ?? throw new KeyNotFoundException($"Prep list item {itemId} not found.");

            if (item.IsComplete)
                throw new InvalidOperationException("Item is already complete.");

            using var transaction = await _context.Database.BeginTransactionAsync(0);
            try
            {
                item.IsComplete = true;
                item.CompletedBy = completedBy;
                item.CompletedAt = DateTime.UtcNow;

                _context.PrepListItems.Update(item);
                await _context.SaveChangesAsync();

                await _auditLogServices.LogAsync(
                    tenantId,
                    completedBy,
                    "force_complete_item",
                    "prep_list",
                    prepListId,
                    null,
                    JsonSerializer.Serialize(new { itemId = itemId }));

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrepList> ForceCompletePrepListAsync(
            Guid prepListId,
            Guid tenantId,
            Guid completedBy)
        {
            var prepList = await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                ?? throw new KeyNotFoundException($"Prep list {prepListId} not found.");

            if (prepList.IsComplete)
                throw new InvalidOperationException("Prep list is already complete.");

            var hasIncompleteItems = prepList.Items.Any(i => !i.IsComplete);
            if (hasIncompleteItems)
                throw new InvalidOperationException("All items must be completed before completing theh prep list.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                prepList.IsComplete = true;
                prepList.CompletedAt = DateTime.UtcNow;

                await _prepListRepository.UpdateAsync(prepList);

                await _auditLogServices.LogAsync(
                    tenantId,
                    completedBy,
                    "force_complete",
                    "prep_list",
                    prepListId,
                    null,
                    JsonSerializer.Serialize(new { CompletedAt = prepList.CompletedAt }));

                await transaction.CommitAsync();

                return await _prepListRepository.GetWithItemsAsync(prepListId, tenantId)
                    ?? prepList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
