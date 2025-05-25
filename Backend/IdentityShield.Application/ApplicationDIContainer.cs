using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Domain.Entities;
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

        services.AddScoped<JwtTokenProvider<ShieldUser>>();

        services.AddValidationExceptionHandler();

        services.AddBusinessExceptionHandler();

        services.AddProblemDetails();

        return services;
    }
}
