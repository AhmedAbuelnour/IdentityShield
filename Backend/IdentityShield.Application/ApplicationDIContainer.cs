using Flaminco.ManualMapper.Extensions;
using IdentityShield.Application.Interfaces.Services;
using IdentityShield.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityShield.Application
{
    public static class ApplicationDIContainer
    {
        public static IServiceCollection AddApplicationDIContainer(this IServiceCollection services)
        {
            services.AddMediatR(a => a.RegisterServicesFromAssemblyContaining<ApplicationScanner>());
            services.AddManualMapper<ApplicationScanner>();

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IRealmService, RealmService>();

            return services;
        }
    }
}
