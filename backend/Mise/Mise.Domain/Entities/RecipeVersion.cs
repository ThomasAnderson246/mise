using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeVersion
    {
        public Guid VersionId { get; set; }
        public Guid RecipeId { get; set; }
        public int VersionNumber { get; set; }
        public bool IsDraft { get; set; } = true;
        public bool IsPUblished { get; set; } = false;
        public Guid? PublishedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //navigation
        public RecipeVersion Version { get; set; } = null!;
        public ICollection<StepCheckOff> StepCheckOffs { get; set; } = new List<StepCheckOff>();
    }
}
