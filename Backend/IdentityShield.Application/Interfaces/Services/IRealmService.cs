using ErrorOr;
using IdentityShield.Application.UseCases.Realms.CreateRealm.Commands;

namespace IdentityShield.Application.Interfaces.Services
{
    public interface IRealmService
    {
        Task<ErrorOr<bool>> CreateAsync(CreateRealmRequest request, CancellationToken cancellationToken = default);
    }
}
