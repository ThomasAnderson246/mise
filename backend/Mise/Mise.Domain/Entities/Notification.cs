using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RecipientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt {  get; set; }

        public Tenant Tenant { get; set; } = null!;
        public User Recipient {  get; set; } = null!;
        public ICollection<NotificationQueue> QueueEntries { get; set; } = new List<NotificationQueue>();
    }
}
