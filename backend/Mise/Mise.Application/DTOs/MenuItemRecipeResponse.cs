using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class MenuItemRecipeResponse
    {
        public Guid MenuItemRecipeId { get; set; }
        public Guid RecipeId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string RecipeStatus { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string? Note {  get; set; }
    }
}
