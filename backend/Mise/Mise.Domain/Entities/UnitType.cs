using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class UnitType
    {
        public Guid UnitTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string MeasureType { get; set; } = string.Empty;
        public decimal? ConversionFactor { get; set; }
        public bool IsNonConvertible { get; set; } = false;


        //navigation
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
