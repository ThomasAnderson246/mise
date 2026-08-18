using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class PrepListItem
    {
        public Guid PrepListItemId { get; set; }
        public Guid PrepListId { get; set; }
        public string SourceType { get; set; } = "custom";
        public string ItemName { get; set; } = string.Empty;
        public Guid? RecipeId { get; set; }
        public decimal? ScalingFactor { get; set; }
        public Guid? AnchorIngredientId { get; set; }
        public decimal? AnchorQuantity {  get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit {  get; set; }
        public string? Notes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsComplete { get; set; } = false;
        public Guid? CompletedBy { get; set; }
        public DateTime? CompletedAt { get; set; }


        // navigation
        public PrepList PrepList { get; set; } = null!;
        public Recipe? Recipe { get; set; }
        public Ingredient? AnchorIngredient { get; set; }
        public User? CompletedByUser { get; set; }

    }
}
