using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeIngredientGroup
    {

        public Guid GroupId { get; set; }
        public Guid VersionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        //navigation

        public RecipeVersion Version { get; set; } = null!;
        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
    }
}
