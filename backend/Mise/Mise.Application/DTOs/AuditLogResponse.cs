using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AuditLogResponse
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? PerformedBy { get; set; }
        public string? PerformedByName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public string? PreviousState {  get; set; }
        public string? NewState { get; set; }
        public string? IpAddress { get; set; }
        public DateTime PerformedAt { get; set; }
    }
}
