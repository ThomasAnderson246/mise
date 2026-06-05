using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class MenuItemAllergen
    {
        public Guid MenuItemAllergenId { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid AllergenId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public Guid? SourceRecipeId { get; set; }
        public string? SourceComponent { get; set; }
        public bool IsDirect { get; set; } = false;
        public bool IsManual { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //navigation
        public MenuItem MenuItem { get; set; } = null!;
        public AllergenTag AllergenTag { get; set; } = null!;
        public Recipe? SourceRecipe { get; set; } 
    }
}
