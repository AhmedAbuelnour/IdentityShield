using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityShield.Infrastructure.Implementations
{

    public class RefreshTokenRepository(ShieldDbContext _dbContext, IOptions<ShieldOptions> _shieldOptions) : IRefreshTokenRepository
    {

        public async Task<string> GenerateRefreshTokenAsync<TUser>(TUser user, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            //using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            //await _dbContext.Set<IdentityRefreshToken>().Where(a => a.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);

            //IdentityRefreshToken refreshToken = new()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            //    UserId = user.Id,
            //    ExpiresOn = DateTime.UtcNow.Add(_shieldOptions.Value.RefreshTokenExpiration),
            //    CreatedOn = DateTime.UtcNow,
            //};

            //_dbContext.Set<IdentityRefreshToken>().Add(refreshToken);

            //await _dbContext.SaveChangesAsync(cancellationToken);

            //await transaction.CommitAsync(cancellationToken);

            // return refreshToken.Token;
            return "";
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<IdentityRefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken) switch
            {
                IdentityRefreshToken refreshToken when refreshToken.IsActive => true,
                _ => false
            };
        }

        public Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _dbContext.Set<IdentityRefreshToken>().Where(a => a.Token == token).Select(a => a.IdentityUser).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> DeleteConsumedTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _dbContext.Set<IdentityRefreshToken>().Where(a => a.Token == token).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
