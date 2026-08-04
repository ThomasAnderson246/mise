using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class SaveDraftRequest
    {
        public List<SaveDraftIngredient> Ingredients { get; set; } = new();
        public List<SaveDraftStep> Steps {  get; set; } = new();
        public List<SaveDraftIngredientGroup> IngredientGroups { get; set; } = new();
    }

    public class SaveDraftIngredient
    {
        public Guid? RecipeIngredientId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? UnitTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public Guid? GroupId { get; set; }
        public bool IsNonConvertible { get; set; }
        public bool IsRatioAnchor { get; set; }

    }

    public class SaveDraftStep
    {
        public Guid? StepId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public bool HasTimer { get; set; }
        public int? TimerDuration { get; set; }
        public bool IsAsync { get; set; }
        public Guid? AsyncGroupId { get; set; }
    }

    public class SaveDraftIngredientGroup
    {
        public Guid? GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
