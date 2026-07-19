using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class MenuItemResponse
    {
        public Guid MenuItemId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description {  get; set; } 
        public string? Course { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<MenuItemRecipeResponse> Recipes { get; set; } = new();
        public List<MenuItemAllergenResponse> Allergens { get; set; } = new();
    }
}
