using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts
{
    public interface IRefreshTokenRepository
    {
        Task<ShieldRefreshToken> GenerateRefreshTokenAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<ShieldUser?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<int> DeleteConsumedTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> CheckActiveSessionAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<int> LogoutAsync(string token, CancellationToken cancellationToken);
        Task<ShieldRefreshToken?> GetNonRevokedRefreshTokenAsync(IdentityUser user, CancellationToken cancellationToken);
    }
}
