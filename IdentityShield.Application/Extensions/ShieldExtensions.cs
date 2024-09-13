using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityShield.Application.Extensions
{
    public static class ShieldExtensions
    {

        public static IServiceCollection AddShieldJwtBearerAuthentication(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          string sectionName = "Shield")
        {
            ShieldOptions shieldOptions = new();

            configuration.GetSection(sectionName).Bind(shieldOptions);

            services.Configure<ShieldOptions>(configuration.GetSection(sectionName));

            shieldOptions.Validate();

            return AddShieldOptionsJwtBearerAuthenticationInternal(services, shieldOptions);
        }

        public static IServiceCollection AddShieldJwtBearerAuthentication(this IServiceCollection services, Action<ShieldOptions> configureOptions)
        {
            ShieldOptions shieldOptions = new();

            configureOptions(shieldOptions);

            services.Configure(configureOptions);

            shieldOptions.Validate();

            return AddShieldOptionsJwtBearerAuthenticationInternal(services, shieldOptions);
        }

        private static IServiceCollection AddShieldOptionsJwtBearerAuthenticationInternal(IServiceCollection services, ShieldOptions shieldOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudiences = shieldOptions.Audiences,
                    ValidIssuer = shieldOptions.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(shieldOptions.SecretKey)),
                    RoleClaimType = shieldOptions.RoleClaimType,
                    NameClaimType = shieldOptions.NameClaimType,
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (!string.IsNullOrEmpty(context.Request.Query["access_token"]))
                        {
                            // Read the token out of the query string
                            context.Token = context.Request.Query["access_token"];
                        }
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }

                        if (context.Exception.GetType() == typeof(SecurityTokenValidationException))
                        {
                            context.Response.Headers["Token-Validation"] = "false";
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

    }
}
