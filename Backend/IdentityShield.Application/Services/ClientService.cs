using ErrorOr;
using Flaminco.ManualMapper.Abstractions;
using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Application.Interfaces.Services;
using IdentityShield.Application.UseCases.Clients.CreateClient.Commands;
using IdentityShield.Domain.Constants;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Services
{
    public class ClientService(IClientRepository _clientRepo, IMapper _mapper) : IClientService
    {
        public async Task<ErrorOr<bool>> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken)
        {
            Client client = _mapper.Map<CreateClientRequest, Client>(request);

            if (await _clientRepo.IsDuplicatedAsync(client, cancellationToken))
            {
                return Error.Validation("duplicated client", "The system found an existed client with the same client id");
            }

            int effectedRows = await _clientRepo.CreateAsync(client, cancellationToken);

            if (effectedRows == Constant.AppConstants.NoEffectedRows)
            {
                return Error.Validation("no effect", "The system didn't make the required change");
            }

            return true;
        }
    }
}
