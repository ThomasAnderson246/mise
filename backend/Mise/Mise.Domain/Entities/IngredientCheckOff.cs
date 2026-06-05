using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class IngredientCheckOff
    {
        public Guid CheckOffId { get; set; }
        public Guid SessionId { get; set; }
        public Guid RecipeIngredientId { get; set; }
        public Guid? CheckedBy { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
        public bool IsComplete { get; set; } = true;

        //navigation
        public CookingSession Session { get; set; } = null!;
        public RecipeIngredient RecipeIngredient { get; set; } = null!;
        public User? CheckedByUser { get; set; }
    }
}
