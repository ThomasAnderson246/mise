using Mise.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mise.Application.Interfaces;

namespace Mise.Infrastructure.Services
{
    public class CurrentUserServices : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServices (IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public Guid TenantId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User
                    .FindFirstValue("tenantId");
                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Email => _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public string Role => _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User
                .Identity?.IsAuthenticated ?? false;
    }
}
