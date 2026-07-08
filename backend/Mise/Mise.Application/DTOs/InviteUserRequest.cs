using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class InviteUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UnitPreference { get; set; } = "metric";
        public List<Guid>? RoleIds { get; set; }
    }
}
