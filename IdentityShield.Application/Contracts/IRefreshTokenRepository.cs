using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts
{
    public interface IRefreshTokenRepository
    {
        Task<string> GenerateRefreshTokenAsync<TUser>(TUser user, CancellationToken cancellationToken) where TUser : IdentityUser;
        Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<int> DeleteConsumedTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
    }
}
