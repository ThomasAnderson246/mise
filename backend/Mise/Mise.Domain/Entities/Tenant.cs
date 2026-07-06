using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
	public class Tenant
	{

        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColour { get; set; }
        public string? SecondaryColour { get;set; }
        public string Tier { get; set; } = "base";
        public string DefaultUnitSystem { get; set; } = "imperial";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<PrepList> PrepLists { get; set; } = new List<PrepList>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<AllergenTag> AllergenTags { get; set; } = new List<AllergenTag>();
	}
}
