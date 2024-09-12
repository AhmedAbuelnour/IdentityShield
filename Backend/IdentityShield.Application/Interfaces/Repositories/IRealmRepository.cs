using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Interfaces.Repositories
{
    public interface IRealmRepository
    {
        Task<int> CreateAsync(Realm realm, CancellationToken cancellationToken);
        Task<bool> IsDuplicatedAsync(Realm realm, CancellationToken cancellationToken);
    }
}
