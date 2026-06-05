using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class NotificationQueue
    {
        public Guid QueueId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RecipientId { get; set; }
        public Guid NotificationId { get; set; }
        public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
        public bool IsDelivered { get; set; } = false;

        public Tenant Tenant { get; set; } = null!;
        public User Recipient { get; set; } = null!;
        public Notification Notification { get; set; } = null!;
    }
}
