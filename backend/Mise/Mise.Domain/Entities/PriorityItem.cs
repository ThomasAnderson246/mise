using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class PriorityItem
    {

        public Guid PriorityItemId { get; set; }
        public Guid TenantId { get; set; }

        public string SourceType { get; set; } = "recipe";
        public Guid? RecipeId { get; set; }
        public string ItemName { get; set; } = string.Empty;

        public decimal? ScalingFactor { get; set; }
        public Guid? AnchorIngredientId { get; set; }
        public decimal? AnchorQuantity { get; set; }
        public string? Notes { get; set; }

        public string Origin { get; set; } = "manual";
        public string ReasonCode { get; set; } = string.Empty;
        public string? ReasonNote { get; set; }
        public Guid FlaggedBy { get; set; }

        public Guid? SourcePrepListId { get; set; }
        public Guid? SourcePrepListItemId { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // navigation
        public Tenant Tenant { get; set; } = null!;
        public Recipe? Recipe { get; set; }
        public Ingredient? AnchorIngredient { get; set; }
        public User FlaggedByUser { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public PrepList? SourcePrepList { get; set; }
        public PrepListItem? SourcePrepListItem { get; set; }
    }
}
