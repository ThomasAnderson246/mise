using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Infrastructure.Persistence.Context;
using Mise.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Mise.Application;
using Mise.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mise.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<MiseDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            // repositories will get registered here as they're built
            services.AddScoped<ITenantRepositoryService, TenantRepository>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // application services will be registered here as they're built
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthService, AuthServices>();
            services.AddScoped<ICurrentUserService, CurrentUserServices>();

            //rbac
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                });

            return services;
        }
    }
        
    
}
