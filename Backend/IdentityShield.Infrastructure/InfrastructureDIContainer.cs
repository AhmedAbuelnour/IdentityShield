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

        services.AddShieldJwtBearerAuthentication(options =>
        {
            options.Audiences = ["Attachment", "Chat", "Curriculum", "Identity", "Dashboard", "Notifier", "Payment", "Student", "Teacher"];
            options.Issuer = "IdentityShield";
            options.SecretKey = "2LtzaU5F2srfjunV+MRDpBAoj/LqJWb6YEhxiAZU7XY=";
            options.RoleClaimType = "roles";
            options.NameClaimType = "name";
            options.SessionClaimType = "session";
            options.AccessTokenExpiration = TimeSpan.FromMinutes(60);
            options.RefreshTokenExpiration = TimeSpan.FromDays(7);
            options.OTPTokenExpiry = TimeSpan.FromMinutes(30);
            options.ConnectionString = configuration["Shield:ConnectionString"];
            options.HashKey = "SelahElTelmeez";
            options.LockoutOptions = new LockoutOptions
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                MaxFailedAccessAttempts = 5
            };
            options.EmailOptions = new EmailOptions
            {
                MailFrom = "noreply@selaheltelmeez.com",
                DisplayName = "Selaheltelmeez - سلاح التلميذ",
                Host = "smtp.office365.com",
                Password = "1ld03J7sT1HeTRlFro$Aswi!rOqatHuGLZLBadrE"
            };
        });

        return services;
    }
}
