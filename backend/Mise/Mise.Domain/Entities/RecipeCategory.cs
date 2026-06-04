using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeCategory
    {
        public Guid RecipeId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
