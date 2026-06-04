using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class CookingSession
    {
        public Guid SessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RecipeId { get; set; }
        public Guid VersionId { get; set; }
        public Guid? StartedBy { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
