using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class RefreshTokenRequest
    {
        public Guid TenantId { get; set; }
    }
}
