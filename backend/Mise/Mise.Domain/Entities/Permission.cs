using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string Name { get; set; } = string.Empty;  
        public string? Description { get; set; }
        public string Resource { get; set; } = string.Empty;
        public string Action {  get; set; } = string.Empty;

    }
}
