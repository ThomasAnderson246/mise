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

        //navigation
        public Tenant Tenant { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
        public RecipeVersion Version { get; set; } = null!;
        public User? StartedByUser { get; set; }
        public ICollection<StepCheckOff> StepCheckOffs { get; set; } =  new List<StepCheckOff>();
        public ICollection<IngredientCheckOff> IngredientCheckOffs { get; set; } = new List<IngredientCheckOff>();
    }
}
