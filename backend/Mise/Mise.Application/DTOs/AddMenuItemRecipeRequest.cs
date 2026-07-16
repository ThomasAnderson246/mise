using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AddMenuItemRecipeRequest
    {
        public Guid RecipeId { get; set; }
        public int DisplayOrder {  get; set; }
        public string? Note { get; set; }
    }
}
