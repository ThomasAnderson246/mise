using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mise.Infrastructure.Persistence.Context;

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
            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // application services will be registered here as they're built
            return services;
        }
    }
        
    
}
