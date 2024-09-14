using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Domain.Entities;
using IdentityShield.Infrastructure.Persistence;
using IdentityShield.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;


/*
 * Id_Token is for frontend that can contains user information the frontend can extract and read its data (email, address, firstname,...)
 * Access_token it should only contains the most basic data for authenticating a user like (role, audience, userId)
 * Both of them have the same expiry date. 
 * 
 * 
 
 */
namespace IdentityShield.Infrastructure
{
    public static class InfrastructureDIContainer
    {
        public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer("Server=AHMED\\SQLEXPRESS;Database=OpenIddictDb;Trusted_Connection=True;TrustServerCertificate=True;");

                // Register the entity sets needed by OpenIddict but use the specified entities instead of the default ones.
                options.UseOpenIddict<ShieldClient, ShieldAuthorization, ShieldScope, ShieldToken, Guid>();
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            services.AddAuthorization();

            services.AddOpenIddict()

            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>()
                       .ReplaceDefaultEntities<ShieldClient, ShieldAuthorization, ShieldScope, ShieldToken, Guid>();        // Configure OpenIddict to use the custom entities.

            }).AddServer(options =>
            {
                options.SetTokenEndpointUris("/connect/token");

                options.AllowPasswordFlow();

                options.AllowClientCredentialsFlow();

                // Enable the refresh token flow.
                options.AllowRefreshTokenFlow();

                // Disable sliding expiration so refresh tokens have a fixed expiration time.
                options.DisableSlidingRefreshTokenExpiration();

                // Set the refresh token to be used only once.
                options.SetRefreshTokenReuseLeeway(TimeSpan.Zero);

                //    options.DisableAccessTokenEncryption();

                options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();

                // Set token lifetime (optional).
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                // Register the ASP.NET Core integration for OpenIddict.
                options.UseAspNetCore().EnableTokenEndpointPassthrough();

                options.RegisterScopes(OpenIddictConstants.Scopes.Roles); // Enable roles

                options.RegisterClaims(OpenIddictConstants.Claims.Role);

            }).AddValidation(options =>
            {
                // Configure the OpenIddict validation handler to use the same configuration
                // as the OpenIddict server (for example, using the same signing key).
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();

            });

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            services.AddMediatR(a => a.RegisterServicesFromAssemblyContaining<ApplicationDbContext>());

            services.AddScoped<IRealmRepository, RealmRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();

            return services;
        }
    }
}
