using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<int> CreateAsync(Client client, CancellationToken cancellationToken);
        Task<bool> IsDuplicatedAsync(Client client, CancellationToken cancellationToken);
    }
}
