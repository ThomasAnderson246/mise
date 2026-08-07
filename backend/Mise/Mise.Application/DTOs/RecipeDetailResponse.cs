using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RecipeDetailResponse
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ScalingMode {  get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RecipeCategoryResponse> RecipeCategories { get; set; } = new();
        public RecipeVersionResponse? CurrentVersion { get; set; }
    }

    public class RecipeCategoryResponse
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RecipeVersionResponse
    {
        public Guid VersionId { get; set; }
        public int VersionNumber { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPublished { get; set; }
        public List<RecipeIngredientGroupResponse> RecipeIngredientGroups { get; set; } = new();
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<RecipeStepResponse> Steps { get; set; } = new();
    }

    public class RecipeIngredientGroupResponse
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientResponse
    {
        public Guid RecipeIngredientId { get; set; }
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? UnitName { get; set; }
        public Guid? UnitTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public Guid? GroupId { get; set; }
        public bool IsRatioAnchor { get; set; }
        public bool IsNonConvertible { get; set; }
    }

    public class RecipeStepResponse
    {
        public Guid StepId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public bool HasTimer { get; set; }
        public int? TimerDuration { get; set; }
        public bool IsAsync     { get; set; }
        public Guid? AsyncGroupId { get; set; }
    }
}
