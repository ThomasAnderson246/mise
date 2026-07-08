using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class InviteUserResponse
    {
        public UserResponse User { get; set; } = null!;
        public string TemporaryPassword { get; set; } = string.Empty;
    }
}
