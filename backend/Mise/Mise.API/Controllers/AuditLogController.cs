using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;


namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogServices _auditLogService;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogController(IAuditLogServices auditLogService, ICurrentUserService currentUserService)
        {
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [RequiresPermission("audit", "read")]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = _currentUserService.TenantId;



            var logs = await _auditLogService.GetByTenantAsync(tenantId);

            var response = logs.Select(a => new AuditLogResponse
            {
                AuditLogId = a.AuditLogId,
                TenantId = a.TenantId,
                PerformedBy = a.PerformedBy,
                PerformedByName = a.PerformedByUser != null
                    ? $"{a.PerformedByUser.FirstName} {a.PerformedByUser.LastName}"
                    : null,
                Action = a.Action,
                Resource = a.Resource,
                ResourceId = a.ResourceId,
                PreviousState = a.PreviousState,
                NewState = a.NewState,
                IpAddress = a.IpAddress,
                PerformedAt = a.PerformedAt,

            });
            return Ok(ApiResponse<IEnumerable<AuditLogResponse>>.Ok(response));
        }

        [HttpGet("{resource}/{resourceId}")]
        [RequiresPermission("audit", "read")]
        public async Task<IActionResult> GetByResource(string resource, Guid resourceId)
        {
            var tenantId = _currentUserService.TenantId;

            var logs = await _auditLogService.GetByResourceAsync(tenantId, resource, resourceId);

            var response = logs.Select(a => new AuditLogResponse
            {
                AuditLogId = a.AuditLogId,
                TenantId = a.TenantId,
                PerformedBy = a.PerformedBy,
                PerformedByName = a.PerformedByUser != null
                    ? $"{a.PerformedByUser.FirstName} {a.PerformedByUser.LastName}"
                    : null,
                Action = a.Action,
                Resource = a.Resource,
                ResourceId = a.ResourceId,
                PreviousState = a.PreviousState,
                NewState = a.NewState,
                IpAddress = a.IpAddress,
                PerformedAt = a.PerformedAt,
            });

            return Ok(ApiResponse<IEnumerable<AuditLogResponse>>.Ok(response));
        }
    }
}
