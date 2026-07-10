using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RoleResponse
    {
        public Guid RoleId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; } = false;
        public List<string> Permissions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
