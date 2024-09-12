using Flaminco.ManualMapper.Abstractions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.UseCases.Clients.CreateClient.Commands
{
    public class CreateClientRequestMapper : IMapHandler<CreateClientRequest, Client>
    {
        public Client Handler(CreateClientRequest source)
        {
            return new Client
            {
                RealmId = source.RealmId,
                ClientId = source.ClientId,
                GrantType = source.GrantType,
                ClientSecret = source.ClientSecret,
                RedirectUri = source.RedirectUri
            };
        }
    }
}
