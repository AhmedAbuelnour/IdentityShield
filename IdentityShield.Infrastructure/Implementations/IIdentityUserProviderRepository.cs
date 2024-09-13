using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Implementations
{

    public class IdentityUserProviderRepository(ShieldDbContext _dbContext) : IIdentityUserProviderRepository
    {

        public Task<bool> CheckUniquenessAsync(string name, string value, CancellationToken cancellationToken)
        {
            return _dbContext.Set<IdentityUserProvider>().Where(a => a.Name == name && a.Value == value).AnyAsync(cancellationToken);
        }

        public Task<string?> GetUserIdAsync(string name, string value, CancellationToken cancellationToken)
        {
            return _dbContext.Set<IdentityUserProvider>().Where(a => a.Name == name && a.Value == value).Select(a => a.UserId).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> LinkProviderAsync<TUser>(TUser user, string name, string value, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            _dbContext.Set<IdentityUserProvider>().Add(new IdentityUserProvider
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Name = name,
                Value = value,
                CreatedOn = DateTime.UtcNow,
            });

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> UnlinkProviderAsync<TUser>(TUser user, string name, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            return _dbContext.Set<IdentityUserProvider>().Where(a => a.UserId == user.Id && a.Name == name).ExecuteDeleteAsync(cancellationToken);
        }

        public Task<int> GetTokenCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            DateTime now = DateTime.UtcNow;

            DateTime startTime = now - duration;

            return _dbContext.Set<TokenTracker>()
                             .Where(a => a.UserId == user.Id)
                             .Where(a => a.Purpose == purpose)
                             .Where(a => a.Provider == provider)
                             .Where(a => a.CreatedOn >= startTime && a.CreatedOn <= now)
                             .CountAsync(cancellationToken);

        }

        public Task<int> AddTokenAsync<TUser>(TUser user, string purpose, string token, string provider, CancellationToken cancellationToken) where TUser : IdentityUser
        {

            _dbContext.Set<TokenTracker>().Add(new TokenTracker
            {
                CreatedOn = DateTime.UtcNow,
                Id = token,
                UserId = user.Id,
                Purpose = purpose,
                Provider = provider,
            });

            return _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}
