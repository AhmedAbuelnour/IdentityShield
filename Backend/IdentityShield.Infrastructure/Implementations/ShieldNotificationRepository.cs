using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Implementations
{
    public sealed class ShieldNotificationRepository(ShieldDbContext _dbContext) : IShieldNotificationRepository
    {
        public Task<int> GetSentCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            DateTime now = DateTime.UtcNow;

            DateTime startTime = now - duration;

            return _dbContext.Set<ShieldNotification>()
                             .Where(a => a.UserId == user.Id)
                             .Where(a => a.Purpose == purpose)
                             .Where(a => a.Provider == provider)
                             .Where(a => a.CreatedAt >= startTime && a.CreatedAt <= now)
                             .CountAsync(cancellationToken);

        }

        public Task<int> AddAsync(ShieldNotification shieldNotification, CancellationToken cancellationToken)
        {
            _dbContext.Set<ShieldNotification>().Add(shieldNotification);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
