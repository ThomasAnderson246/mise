using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class User
    {

        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string FirstName {  get; set; }= string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public string UnitPreference { get; set; } = "metric";
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;

        // navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
