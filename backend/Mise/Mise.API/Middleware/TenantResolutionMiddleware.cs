using Mise.Application.Interfaces;
using Mise.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Mise.API.Middleware
{
    public class TenantResolutionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(
            RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentUserService currentUser,
            MiseDbContext dbContext)
        {
            // skip resolution for unauthenticated endpoints
            if (!currentUser.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var tenantId = currentUser.TenantId;

            if (tenantId == Guid.Empty)
            {
                _logger.LogWarning("Authenticated request missing tenantId claim.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[] { "Tenant could not be resolves" }
                });
                return;
            }

            //verify tenant exists and is active

            var tenant = await dbContext.Tenants
                .FirstOrDefaultAsync(t => 
                    t.TenantId == tenantId && 
                    t.IsActive);

            if (tenant == null)
            {
                _logger.LogWarning("Request made for inactive or non-existent tenant.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[] { "Tenant not found or inactive." }
                });
                return;
            }

            //tenant is valid... continue
            await _next(context);
        }
    }
}
