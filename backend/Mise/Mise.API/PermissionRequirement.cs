using Microsoft.AspNetCore.Authorization;

namespace Mise.API
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Resource { get; }
        public string Action { get; }

        public PermissionRequirement(string resource, string action)
        {
            Resource = resource;
            Action = action;
        }
    }
}
