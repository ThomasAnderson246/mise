using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class PrepListItemResponse
    {
        public Guid PrepListItemId { get; set; }
        public Guid PrepListId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public Guid? RecipeId { get; set; }
        public string? RecipeTitle { get; set; }
        public decimal? ScalingFactor { get; set; }
        public Guid? AnchorIngredientId { get; set; }
        public string? AnchorIngredientName { get; set; }
        public decimal? AnchorQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit {  get; set; }
        public string? Notes { get; set; }
        public int DisplayOrder {  get; set; }
        public bool IsComplete { get; set; }
        public Guid? CompletedBy { get; set; }
        public string? CompletedByName { get; set; }
        public DateTime? CompletedAt { get; set; }

    }
}
