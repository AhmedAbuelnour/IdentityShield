using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts
{
    public interface IIdentityUserProviderRepository
    {
        Task<bool> CheckUniquenessAsync(string name, string value, CancellationToken cancellationToken);
        Task<int> LinkProviderAsync<TUser>(TUser user, string name, string value, CancellationToken cancellationToken) where TUser : IdentityUser;
        Task<int> UnlinkProviderAsync<TUser>(TUser user, string name, CancellationToken cancellationToken) where TUser : IdentityUser;
        Task<string?> GetUserIdAsync(string name, string value, CancellationToken cancellationToken);
        Task<int> GetTokenCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration, CancellationToken cancellationToken) where TUser : IdentityUser;
        Task<int> AddTokenAsync<TUser>(TUser user, string purpose, string token, string provider, CancellationToken cancellationToken) where TUser : IdentityUser;
    }
}
