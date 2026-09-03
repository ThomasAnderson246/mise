using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RecipeListResponse
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public string Status { get; set; } = string.Empty;
        public string ScalingMode {  get; set; } = string.Empty;
        public bool IsPortion { get; set; }
        public bool HasActiveDraft { get; set; }
        public int IngredientCount { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RecipeCategoryResponse> RecipeCategories { get; set; } = new();
    }
}
