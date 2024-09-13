using IdentityShield.Domain.Entities;
using IdentityShield.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        public DbSet<Realm> Realms => Set<Realm>();
        public DbSet<PermissionLookup> PermissionLookups => Set<PermissionLookup>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
            .HasOne(u => u.Realm)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RealmId)
            .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete

            builder.Entity<ApplicationRole>()
                .HasOne(r => r.Realm)
                .WithMany(re => re.Roles)
                .HasForeignKey(r => r.RealmId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete


            builder.ApplyConfigurationsFromAssembly(typeof(IConfigurationScanner).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
