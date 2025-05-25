using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace IdentityShield.Infrastructure.Implementations
{

    public class RefreshTokenRepository(ShieldDbContext _dbContext, IOptions<ShieldOptions> _shieldOptions) : IRefreshTokenRepository
    {

        public async Task<ShieldRefreshToken> GenerateRefreshTokenAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            Guid? sessionId = null;

            if (await _dbContext.Set<ShieldRefreshToken>().Where(a => a.RevokedAt == null && a.UserId == user.Id).FirstOrDefaultAsync(cancellationToken) is ShieldRefreshToken identityRefreshToken
                && identityRefreshToken.SessionId.HasValue)
            {
                sessionId = identityRefreshToken.SessionId;

                _dbContext.Set<ShieldRefreshToken>().Remove(identityRefreshToken);
            }

            ShieldRefreshToken refreshToken = new()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.Add(_shieldOptions.Value.RefreshTokenExpiration),
                CreatedAt = DateTime.UtcNow,
                SessionId = sessionId ?? Guid.NewGuid()
            };

            _dbContext.Set<ShieldRefreshToken>().Add(refreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<ShieldRefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken) switch
            {
                ShieldRefreshToken refreshToken when refreshToken.IsActive => true,
                _ => false
            };
        }

        public Task<ShieldUser?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldRefreshToken>().Where(a => a.Token == token).Select(a => a.IdentityUser).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> DeleteConsumedTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldRefreshToken>().Where(a => a.Token == token).ExecuteDeleteAsync(cancellationToken);
        }

        public Task<bool> CheckActiveSessionAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldRefreshToken>().Where(a => a.UserId == user.Id).AnyAsync(cancellationToken);
        }

        public Task<int> LogoutAsync(string token, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldRefreshToken>().Where(a => a.Token == token).ExecuteDeleteAsync(cancellationToken);
        }

        public Task<ShieldRefreshToken?> GetNonRevokedRefreshTokenAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldRefreshToken>()
                             .Where(a => a.UserId == user.Id)
                             .Where(a => a.RevokedAt == null)
                             .OrderByDescending(a => a.CreatedAt)
                             .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
