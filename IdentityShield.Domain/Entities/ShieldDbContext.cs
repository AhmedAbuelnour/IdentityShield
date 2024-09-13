using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Domain.Entities
{
    public class ShieldDbContext(DbContextOptions<ShieldDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<IdentityUserProvider> IdentityUserProviders => Set<IdentityUserProvider>();
        public DbSet<IdentityRefreshToken> IdentityRefreshTokens => Set<IdentityRefreshToken>();
        public DbSet<TokenTracker> TokenTrackers => Set<TokenTracker>();
    }

}
