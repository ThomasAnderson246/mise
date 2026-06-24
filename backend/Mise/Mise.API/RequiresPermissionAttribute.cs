using Microsoft.AspNetCore.Authorization;

namespace Mise.API
{
    public class RequiresPermissionAttribute : AuthorizeAttribute
    {
        public string Resource { get; }
        public string Action { get; }

        public RequiresPermissionAttribute (string resource, string action)
            : base(policy: $"{resource}.{action}")
        {
            Resource = resource;
            Action = action;
        }
    }
}
