using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class IngredientAllergen
    {
        public Guid IngredientId { get; set; }
        public Guid AllergenId { get; set; }

        //navigation
        public Ingredient Ingredient { get; set; } = null!;
        public AllergenTag AllergenTag { get; set; } = null!;
    }
}
