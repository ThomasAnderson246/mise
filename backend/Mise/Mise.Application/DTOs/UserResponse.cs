using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UserResponse
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
        public string UnitPreference {  get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = false;
        public List<string> Roles { get; set; } = new();
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
