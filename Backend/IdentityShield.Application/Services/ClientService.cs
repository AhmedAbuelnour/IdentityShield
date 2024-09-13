using ErrorOr;
using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Application.Interfaces.Services;
using IdentityShield.Application.UseCases.Clients.CreateClient.Commands;
using IdentityShield.Domain.Entities;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace IdentityShield.Application.Services
{
    public class ClientService(ILookupRepository _lookupRepo, OpenIddictApplicationManager<ShieldClient> _applicationManager) : IClientService
    {
        public async Task<ErrorOr<bool>> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken)
        {
            OpenIddictApplicationDescriptor clientDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                DisplayName = request.DisplayName
            };

            if (request.RedirectUris?.Any() ?? false)
            {
                foreach (string uri in request.RedirectUris)
                {
                    clientDescriptor.RedirectUris.Add(new Uri(uri));
                }
            }

            List<PermissionLookup> permissionLookups = await _lookupRepo.GetPermissionLookupsAsync(cancellationToken);

            if (request.Permissions?.Any() ?? false)
            {
                foreach (Guid permission in request.Permissions)
                {
                    if (permissionLookups.FirstOrDefault(a => a.Id == permission) is PermissionLookup lookup)
                    {
                        clientDescriptor.Permissions.Add(lookup.Value);
                    }
                }
            }

            ShieldClient client = await _applicationManager.CreateAsync(clientDescriptor, cancellationToken);

            client.RealmId = request.RealmId; // Set the realm ID

            await _applicationManager.UpdateAsync(client, cancellationToken); // Save the changes

            return true;
        }
    }
}
