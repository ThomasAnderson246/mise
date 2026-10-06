using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;
using Mise.Application.DTOs;


namespace Mise.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetForUserAsync(Guid userId, Guid tenantId);
        Task<IEnumerable<Notification>> GetUnreadForUserAsync(Guid userId, Guid tenantId);
        Task<Notification> SendDirectMessageAsync(Guid recipientId, string message, Guid tenantId, Guid sentBy);
        Task SendSystemMessageAsync(string title, string message, Guid tenantId, Guid sentBy);
        Task NotifyRecipeUpdatedAsync(Guid recipeId, string reipetitle, Guid tenantId, Guid updatedBy);
        Task NotifyRecipePublishedAsync(Guid recipeId, string recipeTitle, Guid tenantId, Guid publishedBy);
        Task NotifyPrepListAssignedAsync(Guid prepListId, string prepListName, Guid assignedTo, Guid tenantId, Guid assignedBy);
        Task NotifyPrepListIncompleteAsync(string prepListName, string completedByName, IReadOnlyList<IncompleteItemSummary> items, Guid tenantId, Guid completedBy);
        Task MarkAsReadAsync(Guid notificationId, Guid userId, Guid tenantId);
        Task MarkAllAsReadAsync(Guid userId, Guid tenantId);

    }
}
