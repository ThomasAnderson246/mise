using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class PrepList
    {
        public Guid PrepListId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public bool IsComplete { get; set; } = false;
        public Guid? AssignedTo { get; set; }
        public User? AssignedToUser { get; set; }

        // navigation
        public Tenant Tenant { get; set; } = null!;
        public User? CreatedByUser { get; set; }
        public ICollection<PrepListItem> Items { get; set; } = new List<PrepListItem>();
    }
}
