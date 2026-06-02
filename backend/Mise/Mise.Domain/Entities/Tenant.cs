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
        public string Name { get; set; }
        public string Slug {  get; set; }
        public string? LogoUrl { get; set; }
        public string? PrimaryColour { get; set; }
        public string? SecondaryColour { get;set; }
        public string Tier { get; set; } = "base";
        public string DefaultUnitSystem { get; set; } = "imperial";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
	}
}
