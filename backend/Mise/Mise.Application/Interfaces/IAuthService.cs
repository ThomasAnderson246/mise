using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(User user, string roleName);
        Task<User?> ValidateUserAsync(string email, string password, Guid tenantId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
