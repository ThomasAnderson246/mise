using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class MenuItem
    {
        public Guid MenuItemId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Course { get; set; }
        public string Status { get; set; } = "draft";
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt {  get; set; } = DateTime.UtcNow;

        // navigation
        public Tenant Tenant { get; set; } = null!;
        public User? CreatedByUser { get; set; }
        public ICollection<MenuItemRecipe> MenuItemRecipes { get; set; } = new List<MenuItemRecipe>();
        public ICollection<MenuItemAllergen> MenuItemAllergens { get; set; } = new List<MenuItemAllergen>();
    }
}
