using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.Register.Commands;

namespace IdentityShield.API.Modules
{
    public class RegisterModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Register").AllowAnonymous().WithTags("Register");

            group.MediatePost<RegisterByEmailCommand.Command>("/Email").WithName("Register-Email");
            group.MediatePost<ConfirmRegisterEmailCommand.Command>("/Email/Confirm").WithName("Register-Email-Confirm");

            group.MediatePost<RegisterByPhoneNumberCommand.Command>("/PhoneNumber").WithName("Register-PhoneNumber");
            group.MediatePost<ConfirmRegisterPhoneNumberCommand.Command>("/PhoneNumber/Confirm").WithName("Register-PhoneNumber-Confirm");

            group.MediatePost<RegisterByProviderCommand.Command>("/Provider").WithName("Register-Provider");
        }
    }
}
