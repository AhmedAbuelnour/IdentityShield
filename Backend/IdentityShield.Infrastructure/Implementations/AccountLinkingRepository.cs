using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Implementations
{
    public sealed class AccountLinkingRepository(ShieldDbContext _dbContext) : IAccountLinkingRepository
    {
        public Task<int> LinkAsync(string managerAccountId, string managedAccountId, string linkType, CancellationToken cancellationToken)
        {
            ShieldAccountLink accountLink = new()
            {
                ManagerAccountId = managerAccountId,
                ManagedAccountId = managedAccountId,
                LinkType = linkType,
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.Set<ShieldAccountLink>().Add(accountLink);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }


        public Task<int> ReactivateLinkAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldAccountLink>()
                             .Where(link => link.ManagerAccountId == managerAccountId && link.ManagedAccountId == managedAccountId)
                             .ExecuteUpdateAsync(a => a.SetProperty(p => p.IsActive, true), cancellationToken);
        }

        public Task<int> UnlinkAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldAccountLink>()
                             .Where(link => link.ManagerAccountId == managerAccountId && link.ManagedAccountId == managedAccountId)
                             .ExecuteUpdateAsync(a => a.SetProperty(p => p.IsActive, false), cancellationToken);
        }

        public Task<bool> IsActiveLinkedAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldAccountLink>()
                             .Where(link => link.ManagerAccountId == managerAccountId && link.ManagedAccountId == managedAccountId && link.IsActive)
                             .AnyAsync(cancellationToken);
        }

        public Task<bool> IsDisabledLinkedAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<ShieldAccountLink>()
                             .Where(link => link.ManagerAccountId == managerAccountId && link.ManagedAccountId == managedAccountId && link.IsActive == false)
                             .AnyAsync(cancellationToken);
        }

        public async Task<IEnumerable<ShieldUser>> GetManagedAccountsAsync(string managerAccountId, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<ShieldAccountLink>()
                                   .Where(link => link.ManagerAccountId == managerAccountId && link.IsActive)
                                   .Select(link => link.ManagedAccount)
                                   .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ShieldUser>> GetManagedByAccountsAsync(string managerAccountId, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<ShieldAccountLink>()
                                   .Where(link => link.ManagedAccountId == managerAccountId && link.IsActive)
                                   .Select(link => link.ManagerAccount)
                                   .ToListAsync(cancellationToken);
        }
    }
}
