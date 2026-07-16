using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class PrepListResponse
    {
        public Guid PrepListId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PrepListItemResponse> Items { get; set; } = new();
    }
}
