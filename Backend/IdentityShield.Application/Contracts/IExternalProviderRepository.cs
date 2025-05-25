using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts
{

    public interface IExternalProviderRepository
    {
        Task<bool> CheckUniquenessAsync(string name, string value, CancellationToken cancellationToken);
        Task<int> LinkProviderAsync(IdentityUser user, string name, string value, CancellationToken cancellationToken);
        Task<int> UnlinkProviderAsync(IdentityUser user, string name, CancellationToken cancellationToken);
        Task<string?> GetUserIdAsync(string name, string value, CancellationToken cancellationToken);
    }
}
