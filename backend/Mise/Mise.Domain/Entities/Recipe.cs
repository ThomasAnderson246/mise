using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class Recipe
    {
        public Guid RecipeId { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "draft";
        public string ScalingMode { get; set; } = "multiplier";
        public Guid? CurrentVersionId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt {  get; set; } = DateTime.UtcNow;

        //navigation
        public Tenant Tenant { get; set; } = null!;
        public User? CreatedByUser { get; set; }
        public RecipeVersion? CurrentVersion {  get; set; }
        public ICollection<RecipeVersion> Versions { get; set; } = new List<RecipeVersion>();
        public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
        public ICollection<SubRecipe> ParentRecipes { get; set; } = new List<SubRecipe>();
        public ICollection<SubRecipe> ChildRecipes { get; set; } = new List<SubRecipe>();
        public ICollection<MenuItemRecipe> MenuItemRecipes { get; set; } = new List<MenuItemRecipe>();
    }
}
