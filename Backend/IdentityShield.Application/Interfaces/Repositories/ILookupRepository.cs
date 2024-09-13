using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Interfaces.Repositories
{
    public interface ILookupRepository
    {
        Task<List<PermissionLookup>> GetPermissionLookupsAsync(CancellationToken cancellationToken = default);
    }
}
