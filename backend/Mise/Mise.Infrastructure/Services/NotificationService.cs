using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly MiseDbContext _context;

        public NotificationService(MiseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetForUserAsync(Guid userId, Guid tenantId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId && n.TenantId == tenantId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadforUserAsync(Guid userId, Guid tenantId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId && n.TenantId == tenantId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> SendDirectMessageAsync(
            Guid recipientId, 
            string message, 
            Guid tenantId,
            Guid sentBy)
        {
            var sender = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == sentBy)
                ?? throw new KeyNotFoundException("Sender not found.");

            var recipientExists = await _context.Users
                .AnyAsync(u => u.UserId == recipientId && u.TenantId == tenantId);
            if (!recipientExists)
                throw new KeyNotFoundException("Recipient not found.");

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                TenantId = tenantId,
                RecipientId = recipientId,
                Title = $"Message from {sender.FirstName} {sender.LastName}",
                Message = message,
                Type = "direct_message",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await QueueNotificationAsync(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task SendSystemMessageAsync(
            string title,
            string message,
            Guid tenantId,
            Guid sentBy)
        {
            var users = await _context.Users
                .Where(u => u.TenantId == tenantId && u.Status == "active")
                .ToListAsync();

            var notifications = users.Select(u => new Notification
            {
                NotificationId = Guid.NewGuid(),
                TenantId = tenantId,
                RecipientId = u.UserId,
                Title = title,
                Message = message,
                Type = "system_message",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications);

            foreach (var notification in notifications) 
                await QueueNotificationAsync (notification);

            await _context.SaveChangesAsync();
        }

        public async Task NotifyRecipePublishedAsync(
            Guid recipeId,
            string recipeTitle,
            Guid tenantId,
            Guid publishedBy)
        {
            var eligibleUsers = await GetUsersWithPermissionAsync(tenantId, "recipe", "read");

            eligibleUsers = eligibleUsers.Where(u => u.UserId != publishedBy).ToList();

            var notifications = eligibleUsers.Select(U => new Notification
            {
                NotificationId = Guid.NewGuid(),
                TenantId = tenantId,
                RecipientId = U.UserId,
                Title = "New Recipe Published",
                Message = $"{recipeTitle} has been added to the recipe book.",
                Type = "recipe_published",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications);

            foreach (var notification in notifications)
                await QueueNotificationAsync (notification);

            await _context.SaveChangesAsync();
        }

        public async Task NotifyRecipeUpdatedAsync(
            Guid recipeId,
            string recipeTitle,
            Guid tenantId,
            Guid updatedBy)
        {
            var eligibleUsers = await GetUsersWithPermissionAsync(tenantId, "recipe", "read");
            eligibleUsers = eligibleUsers.Where(u => u.UserId != updatedBy).ToList();

            // filters spam, hopefully
            var existingUnread = await _context.Notifications
                .Where(n => n.TenantId == tenantId
                    && n.Type == "recipe_updated"
                    && !n.IsRead
                    && n.Message.Contains(recipeTitle))
                .Select(n => n.RecipientId)
                .ToListAsync();

            eligibleUsers = eligibleUsers
                .Where(u => !existingUnread.Contains(u.UserId))
                .ToList();

            if (!eligibleUsers.Any()) return;

            var notifications = eligibleUsers.Select(u => new Notification
            {
                NotificationId = Guid.NewGuid(),
                TenantId = tenantId,
                RecipientId = u.UserId,
                Title = "Recipe Updated",
                Message = $"{recipeTitle} has been updated. Please review the changes.",
                Type = "recipe_updated",
                IsRead = false,
                CreatedAt = DateTime.UtcNow

            
            });

            await _context.Notifications.AddRangeAsync(notifications);

            foreach (var notification in notifications)
                await QueueNotificationAsync(notification);

            await _context.SaveChangesAsync();

        }

        private async Task QueueNotificationAsync(Notification notification)
        {
            var queueEntry = new NotificationQueue
            {
                QueueId = Guid.NewGuid(),
                TenantId = notification.TenantId,
                RecipientId = notification.RecipientId,
                NotificationId = notification.NotificationId,
                QueuedAt = DateTime.UtcNow,
                IsDelivered = false

            };

            await _context.NotificationsQueues.AddAsync(queueEntry);
        }

        private async Task<List<User>> GetUsersWithPermissionAsync(
            Guid tenantId,
            string resource,
            string action)
        {
            return await _context.Users
                .Where(u => u.TenantId == tenantId && u.Status == "active")
                .Where(u => u.UserRoles.Any(ur =>
                        ur.Role.RolePermissions.Any(rp =>
                            rp.Permission.Resource == resource &&
                            rp.Permission.Action == action)))
                .ToListAsync();
        }

    }
}
