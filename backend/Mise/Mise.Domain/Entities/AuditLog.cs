using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? PerformedBy { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public string? PreviousState { get; set; }
        public string? NewState { get; set; }
        public string? IpAddress { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        public Tenant Tenant { get; set; } = null!;
        public User? PerformedByUser { get; set; } 
    }
}
