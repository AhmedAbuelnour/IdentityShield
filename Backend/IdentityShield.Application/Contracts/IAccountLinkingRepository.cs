
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Contracts
{
    public interface IAccountLinkingRepository
    {
        Task<IEnumerable<ShieldUser>> GetManagedAccountsAsync(string managerAccountId, CancellationToken cancellationToken);
        Task<IEnumerable<ShieldUser>> GetManagedByAccountsAsync(string managerAccountId, CancellationToken cancellationToken);
        Task<bool> IsActiveLinkedAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken);
        Task<bool> IsDisabledLinkedAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken);
        Task<int> LinkAsync(string managerAccountId, string managedAccountId, string linkType, CancellationToken cancellationToken);
        Task<int> ReactivateLinkAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken);
        Task<int> UnlinkAsync(string managerAccountId, string managedAccountId, CancellationToken cancellationToken);
    }
}
