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
        public Guid RecipeId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public int DisplayOrder {  get; set; }
        public decimal ScalingFactor {  get; set; }
        public bool IsComplete { get; set; }
        public Guid? CompletedBy { get; set; }
        public string? CompletedByName { get; set; }
        public DateTime? CompletedAt { get; set; }

    }
}
