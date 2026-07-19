using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class PrepListSummaryResponse
    {
        public Guid PrepListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
