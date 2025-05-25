using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts
{
    public interface IShieldNotificationRepository
    {
        Task<int> AddAsync(ShieldNotification shieldNotification, CancellationToken cancellationToken);
        Task<int> GetSentCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration, CancellationToken cancellationToken) where TUser : IdentityUser;
    }
}
