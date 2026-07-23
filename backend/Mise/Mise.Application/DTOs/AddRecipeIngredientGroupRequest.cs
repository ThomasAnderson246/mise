using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AddRecipeIngredientGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } 
    }
}
