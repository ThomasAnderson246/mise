using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AddPrepListItemRequest
    {
        public string SourceType { get; set; } = "custom";
        public string ItemName { get; set; } = string.Empty;
        public Guid? RecipeId { get; set; }
        public decimal? ScalingFactor { get; set; }
        public Guid? AnchorIngredientId { get; set; }
        public decimal? AnchorQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit {  get; set; }
        public string? Notes { get; set; }
        public int DisplayOrder { get; set; }
    }
}
