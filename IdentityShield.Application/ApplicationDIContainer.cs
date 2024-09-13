using Microsoft.Extensions.DependencyInjection;

namespace IdentityShield.Application;

public static class ApplicationDIContainer
{
    public static IServiceCollection AddApplicationDIContainer(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<IMarkupAssemblyScanning>();
        });

        services.AddScoped<JwtTokenService>();

        return services;
    }
}
