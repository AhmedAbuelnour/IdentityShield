using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Domain.Entities;
using IdentityShield.Infrastructure.Persistence;
using IdentityShield.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>()
                       .ReplaceDefaultEntities<ShieldClient, ShieldAuthorization, ShieldScope, ShieldToken, Guid>();        // Configure OpenIddict to use the custom entities.
            });


            services.AddMediatR(a => a.RegisterServicesFromAssemblyContaining<ApplicationDbContext>());

            services.AddScoped<IRealmRepository, RealmRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();

            return services;
        }
    }
}
