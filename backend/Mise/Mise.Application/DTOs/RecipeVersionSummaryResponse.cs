using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RecipeVersionSummaryResponse
    {
        public Guid VersionId { get; set; }
        public int VersionNumber { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPublished { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? PublishedByName { get; set; }
    }
}
