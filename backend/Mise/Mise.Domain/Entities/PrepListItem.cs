using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class PrepListItem
    {
        public Guid PrepListItemId { get; set; }
        public Guid PrepListId { get; set; }
        public Guid RecipeId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsComplete { get; set; } = false;
        public Guid? CompletedBy { get; set; }
        public DateTime? CompletedAt { get; set; }

        // navigation
        public PrepList PrepList { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
        public User? CompletedByUser { get; set; }

    }
}
