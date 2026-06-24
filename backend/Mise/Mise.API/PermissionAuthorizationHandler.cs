using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.API
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly ICurrentUserService _currentUser;
        private readonly MiseDbContext _dbContext;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(ICurrentUserService currentUser, MiseDbContext dbContext, ILogger<PermissionAuthorizationHandler> logger)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!_currentUser.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // get the user's role first
            var userRole = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(ur => ur.UserId == _currentUser.UserId);

            if (userRole == null)
            {
                _logger.LogWarning("User {UserId} has no role assigned", _currentUser.UserId);
                context.Fail();
                return;
            }

            //check if role has the required permissions
            var hasPermission = userRole.Role.RolePermissions.Any(rp =>
                rp.Permission.Resource == requirement.Resource &&
                rp.Permission.Action == requirement.Action);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning(
                    "User {UserId} with the role {Role} does not have permission {Resource}.{Action}",
                    _currentUser.UserId,
                    userRole.Role.Name,
                    requirement.Resource,
                    requirement.Action);
                context.Fail();
            }
        }
    }
}
