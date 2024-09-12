using ErrorOr;
using IdentityShield.Application.UseCases.Clients.CreateClient.Commands;

namespace IdentityShield.Application.Interfaces.Services
{
    public interface IClientService
    {
        Task<ErrorOr<bool>> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken);
    }
}
