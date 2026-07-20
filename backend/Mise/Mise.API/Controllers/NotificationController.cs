using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NotificationController : ControllerBase
    {

        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public NotificationController(INotificationService notificationService, ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("notification", "read")]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetForUserAsync(_currentUser.UserId, _currentUser.TenantId);

            return Ok(ApiResponse<IEnumerable<NotificationResponse>>.Ok(notifications.Select(
                n => MapToResponse(n))));
        }

        [HttpGet("unread")]
        [RequiresPermission("notification", "read")]
        public async Task<IActionResult> GetUnread()
        {
            var notifications = await _notificationService
                .GetUnreadForUserAsync(_currentUser.UserId, _currentUser.TenantId);

            return Ok(ApiResponse<IEnumerable<NotificationResponse>>.Ok(
                notifications.Select(n => MapToResponse(n))));
        }

        [HttpPost("direct")]
        [RequiresPermission("notification", "send")]
        public async Task<IActionResult> SendDirectMEssage([FromBody] SendDirectMessageRequest request)
        {
            try
            {
                var notification = await _notificationService.SendDirectMessageAsync(
                    request.RecipientId,
                    request.Message,
                    _currentUser.TenantId,
                    _currentUser.UserId);

                return Ok(ApiResponse<NotificationResponse>.Ok(MapToResponse(notification), "Message sent."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<NotificationResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("system")]
        [RequiresPermission("notification", "broadcast")]
        public async Task<IActionResult> SendSystemMessage(
            [FromBody] SendSystemMessageRequest request)
        {
            await _notificationService.SendSystemMessageAsync(
                request.Title,
                request.Message,
                _currentUser.TenantId, _currentUser.UserId);

            return Ok(ApiResponse<string>.Ok("Broadcast sent.", "System message sent."));
        }

        [HttpPost("{id}/read")]
        [RequiresPermission("notification", "read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(
                    id, _currentUser.UserId, _currentUser.TenantId);

                return Ok(ApiResponse<string>.Ok("Marked as read.", "Notification marked as read."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Notification not found."));
            }
        }

        [HttpPost("read-all")]
        [RequiresPermission("notification", "read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(
                _currentUser.UserId, _currentUser.TenantId);

            return Ok(ApiResponse<string>.Ok("All marked as read.", "All notifications marked as read."));
        }

        private static NotificationResponse MapToResponse(Notification N) => new()
        {
            NotificationId = N.NotificationId,
            TenantId = N.TenantId,
            RecipientId = N.RecipientId,
            Title = N.Title,
            Message = N.Message,
            Type = N.Type,
            IsRead = N.IsRead,
            CreatedAt = N.CreatedAt,
            ReadAt = N.ReadAt
        };
    }
}
