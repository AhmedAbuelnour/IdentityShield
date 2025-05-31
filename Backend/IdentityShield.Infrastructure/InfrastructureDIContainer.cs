using IdentityShield.Application.Contracts;
using IdentityShield.Application.Extensions;
using IdentityShield.Application.Shields;
using IdentityShield.Domain.Constants;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using IdentityShield.Infrastructure.Clients;
using IdentityShield.Infrastructure.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityShield.Infrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShieldDbContext>(options =>
        {
            options.UseSqlServer(configuration["Shield:ConnectionString"]);
        });

        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IExternalProviderRepository, ExternalProviderRepository>();

        services.AddScoped<IShieldNotificationRepository, ShieldNotificationRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher<ShieldUser>, ShieldPasswordHasher>();

        services.AddScoped<IIdentityClaimMapper<ShieldUser>, DefaultIdentityClaimMapper>();

        services.AddIdentity<ShieldUser, IdentityRole>(options =>
        {
            LockoutOptions lockoutOptions = new();

            configuration.GetSection("Shield:LockoutOptions").Bind(lockoutOptions);

            options.Lockout = lockoutOptions;
        })
        .AddEntityFrameworkStores<ShieldDbContext>()
        .AddUserManager<ShieldUserManager>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<ShieldOTPTokenProvider<ShieldUser>>(Constant.TokenProviders.OtpTokenProvider);

        services.AddHttpClient<VodafoneSMSClient>();
        services.Configure<VodafoneSMSOptions>(configuration.GetSection("VodafoneSMS"));
        services.Configure<EmailOptions>(configuration.GetSection("EmailSettings"));

        services.Configure<ShieldOptions>(configuration.GetSection("Shield"));

        services.AddShieldJwtBearerAuthentication(configuration.GetSection("Shield").Bind);

        return services;
    }
}
