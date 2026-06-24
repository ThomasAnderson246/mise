using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Mise.API
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {

        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => 
            _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // check if policy name matches our resource.action
            var parts = policyName.Split('.');
            if (parts.Length == 2)
            {
                var resource = parts[0];
                var action = parts[1];

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(resource, action))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // fall back to default policy provider for standard policies

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
