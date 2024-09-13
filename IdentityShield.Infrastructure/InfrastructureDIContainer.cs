using IdentityShield.Application.Contracts;
using IdentityShield.Application.Extensions;
using IdentityShield.Application.Shields;
using IdentityShield.Domain.Constants;
using IdentityShield.Domain.Entities;
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

        services.AddScoped<IIdentityUserProviderRepository, IdentityUserProviderRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher<ApplicationUser>, ShieldPasswordHasher>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            LockoutOptions lockoutOptions = new();

            configuration.GetSection("Shield:LockoutOptions").Bind(lockoutOptions);

            options.Lockout = lockoutOptions;
        })
        .AddEntityFrameworkStores<ShieldDbContext>()
        .AddUserManager<ShieldUserManager>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<ShieldOTPTokenProvider<ApplicationUser>>(Constant.TokenProviders.OtpToken);


        services.AddHttpClient("SMSClient", (handler) =>
        {
            handler.BaseAddress = new Uri(configuration["SMSSettings:BaseUrl"]);
        });

        services.AddShieldJwtBearerAuthentication(configuration);

        return services;
    }
}
