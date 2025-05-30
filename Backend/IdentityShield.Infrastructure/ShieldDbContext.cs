using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpenIddict.EntityFrameworkCore;
using System.Text.Json;

namespace IdentityShield.Infrastructure
{
    public class ShieldDbContext(DbContextOptions<ShieldDbContext> options) : IdentityDbContext<ShieldUser>(options)
    {
        public DbSet<ShieldExternalProvider> ShieldExternalProviders => Set<ShieldExternalProvider>();
        public DbSet<ShieldRefreshToken> ShieldRefreshTokens => Set<ShieldRefreshToken>();
        public DbSet<ShieldNotification> ShieldNotifications => Set<ShieldNotification>();
        public DbSet<ShieldAccountLink> ShieldAccountLinks => Set<ShieldAccountLink>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseOpenIddict();

            // Rename Identity tables
            builder.Entity<ShieldUser>(b => b.ToTable("ShieldUsers"));
            builder.Entity<IdentityRole>(b => b.ToTable("ShieldRoles"));
            builder.Entity<IdentityUserRole<string>>(b => b.ToTable("ShieldUserRoles"));
            builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("ShieldUserClaims"));
            builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("ShieldUserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("ShieldRoleClaims"));
            builder.Entity<IdentityUserToken<string>>(b => b.ToTable("ShieldUserTokens"));

            builder.Entity<ShieldExternalProvider>(b => b.ToTable("ShieldExternalProviders"));
            builder.Entity<ShieldRefreshToken>(b => b.ToTable("ShieldRefreshTokens"));
            builder.Entity<ShieldNotification>(b => b.ToTable("ShieldNotifications"));

            builder.HasDefaultSchema("IdentityShield");



            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true // Optional: makes the JSON easier to read but increases storage use
            };


            builder.Entity<ShieldUser>().Property(a => a.Attributes).HasConversion(v => JsonSerializer.Serialize(v, serializerOptions), v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, serializerOptions) ?? new Dictionary<string, object>(),
              // The following ValueComparer is needed to detect changes correctly since Dictionary<string, object> does not implement IEquatable<>
              new ValueComparer<Dictionary<string, object>>(
                  (c1, c2) => JsonSerializer.Serialize(c1, serializerOptions) == JsonSerializer.Serialize(c2, serializerOptions),
                  c => c == null ? 0 : JsonSerializer.Serialize(c, serializerOptions).GetHashCode(),
                  c => JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(c, serializerOptions), serializerOptions))
          );


            // Configure composite primary key
            builder.Entity<ShieldAccountLink>()
                .HasKey(link => new { link.ManagerAccountId, link.ManagedAccountId });

            // Configure relationships
            builder.Entity<ShieldAccountLink>()
                   .HasOne(link => link.ManagerAccount)
                   .WithMany(user => user.ManagingAccounts)
                   .HasForeignKey(link => link.ManagerAccountId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ShieldAccountLink>()
                   .HasOne(link => link.ManagedAccount)
                   .WithMany(user => user.ManagedByAccounts)
                   .HasForeignKey(link => link.ManagedAccountId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configure default value for LinkType
            builder.Entity<ShieldAccountLink>().Property(link => link.LinkType).HasDefaultValue("Full-Control");
        }
    }
}


