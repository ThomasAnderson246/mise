using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class SubRecipe
    {
        public Guid ParentRecipeId { get; set; }
        public Guid SubRecipeId { get; set; }
    }
}
