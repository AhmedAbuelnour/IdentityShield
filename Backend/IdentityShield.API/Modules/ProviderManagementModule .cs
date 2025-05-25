using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.ExternalProviders.Commands;

namespace IdentityShield.API.Modules
{
    public class ExternalProviderManagementModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Provider").WithTags("Provider-Management");

            group.MediatePost<LinkProviderCommand.Command>("/Link").WithName("Provider-Management-Link");

            group.MediatePost<UnlinkProviderCommand.Command>("/Unlink").WithName("Provider-Management-Unlink");
        }
    }
}
