using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RevokeTokenRequest
    {
        public Guid TenantId { get; set; }
    }
}
