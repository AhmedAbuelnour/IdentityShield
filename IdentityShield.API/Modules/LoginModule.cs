using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.Login.Commands;

namespace IdentityShield.API.Modules
{
    public class LoginModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Login").AllowAnonymous().WithTags("Login");

            group.MediatePost<LoginByEmailCommand.Command>("/Email").WithName("Login-Email");

            group.MediatePost<LoginByPhoneNumberCommand.Command>("/PhoneNumber").WithName("Login-PhoneNumber");

            group.MediatePost<LoginByProviderCommand.Command>("/Provider").WithName("Login-Provider");
        }
    }
}
