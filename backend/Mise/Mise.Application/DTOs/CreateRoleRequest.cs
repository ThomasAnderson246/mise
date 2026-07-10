using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<Guid>? PermissionIds { get; set; }
    }
}
