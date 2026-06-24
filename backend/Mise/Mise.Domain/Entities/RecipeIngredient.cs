using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeIngredient
    {
        public Guid RecipeIngredientId { get; set; }
        public Guid VersionId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? UnitTypeId { get; set; }
        public bool IsNonConvertible { get; set; } = false;
        public bool IsRatioAnchor { get; set; } = false;
        public int DisplayOrder { get; set; }
        public Guid? GroupId { get; set; }

        //navigation
        public RecipeVersion Version { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
        public UnitType? UnitType {get; set; }
        public ICollection<IngredientCheckOff> IngredientCheckOffs { get; set; } = new List<IngredientCheckOff>();
        public RecipeIngredientGroup? Group { get; set; }
    }
}
