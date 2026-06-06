using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class Ingredient
    {
        public Guid IngredientId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public Guid? DefaultUnitTypeId { get; set; }
        public bool IsNonConvertible { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Tenant Tenant { get; set; } = null!;
        public UnitType? DefaultUnitType { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<IngredientAllergen> IngredientAllergens { get; set; } = new List<IngredientAllergen>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
