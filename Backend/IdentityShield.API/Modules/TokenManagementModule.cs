using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.ConsumeRefreshToken.Commands;

namespace IdentityShield.API.Modules
{
    public class TokenManagementModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Token").WithTags("Token-Management");

            group.MediatePost<ConsumeRefreshTokenCommand.Command>("/Refresh").WithName("Token-Management-Refresh");
        }
    }
}
