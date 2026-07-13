using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class SubRecipeResponse
    {
        public Guid ParentRecipeId { get; set; }
        public Guid SubRecipeId { get; set; }
        public string SubRecipeTitle { get; set; } = string.Empty;
        public string SubRecipeStatus { get; set; } = string.Empty;
    }
}
