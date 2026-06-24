using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }

        //navigation
        public User User { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
    }
}
