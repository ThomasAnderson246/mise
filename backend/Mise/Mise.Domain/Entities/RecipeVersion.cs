using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeVersion
    {
        public Guid VersionId { get; set; }
        public Guid RecipeId { get; set; }
        public int VersionNumber { get; set; }
        public bool IsDraft { get; set; } = true;
        public bool IsPublished { get; set; } = false;
        public Guid? PublishedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //navigation
        
        public Recipe Recipe { get; set; } = null!;
        public User? PublishedByUser { get; set; }

        public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeIngredientGroup> IngredientGroups { get; set; } = new List<RecipeIngredientGroup>();
    }
}
