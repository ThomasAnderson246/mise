using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class MenuItemRecipe
    {
        public Guid MenuItemRecipeId { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid RecipeId { get; set; }
        public int DisplayOrder { get; set; }
        public string? Note { get; set; }

        //navigation
        public MenuItem MenuItem { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
    }
}
