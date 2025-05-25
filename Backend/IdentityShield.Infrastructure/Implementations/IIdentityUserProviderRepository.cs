using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Implementations
{
    public sealed class ExternalProviderRepository(ShieldDbContext _dbContext) : IExternalProviderRepository
    {
        public Task<bool> CheckUniquenessAsync(string name, string value, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldExternalProvider>().Where(a => a.Name == name && a.Value == value).AnyAsync(cancellationToken);
        }

        public Task<string?> GetUserIdAsync(string name, string value, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldExternalProvider>().Where(a => a.Name == name && a.Value == value).Select(a => a.UserId).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> LinkProviderAsync(IdentityUser user, string name, string value, CancellationToken cancellationToken)
        {
            _dbContext.Set<ShieldExternalProvider>().Add(new ShieldExternalProvider
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Name = name,
                Value = value,
                CreatedAt = DateTime.UtcNow,
            });

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> UnlinkProviderAsync(IdentityUser user, string name, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldExternalProvider>().Where(a => a.UserId == user.Id && a.Name == name).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
