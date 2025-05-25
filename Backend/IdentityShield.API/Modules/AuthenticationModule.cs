using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.Auth.Login.Commands;
using IdentityShield.Application.Endpoints.Auth.Logout.Commands;
using IdentityShield.Application.Endpoints.Auth.Register.Commands;

namespace IdentityShield.API.Modules
{
    public class AuthenticationModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Authentication").AllowAnonymous().WithTags("Authentication");

            group.MediatePost<LoginByEmailCommand.Command>("/Login/Email").WithName("Authentication-Login-Email");

            group.MediatePost<LoginByPhoneNumberCommand.Command>("/Login/PhoneNumber").WithName("Authentication-Login-PhoneNumber");

            group.MediatePost<LoginByProviderCommand.Command>("/Login/Provider").WithName("Authentication-Login-Provider");

            group.MediatePost<LoginByManagedAccountCommand.Command>("/Login/ManagedAccount").WithName("Authentication-Login-ManagedAccount");

            group.MediatePost<RegisterByEmailCommand.Command>("/Register/Email").WithName("Authentication-Register-Email");
            group.MediatePost<ConfirmRegisterEmailCommand.Command>("/Register/Confirm/Email").WithName("Authentication-Register-Email-Confirm");

            group.MediatePost<RegisterByPhoneNumberCommand.Command>("/Register/PhoneNumber").WithName("Authentication-Register-PhoneNumber");
            group.MediatePost<ConfirmRegisterPhoneNumberCommand.Command>("/Register/Confirm/PhoneNumber").WithName("Authentication-Register-PhoneNumber-Confirm");

            group.MediatePost<RegisterByProviderCommand.Command>("/Register/Provider").WithName("Authentication-Register-Provider");

            group.MediatePost<LogoutCommand.Command>("/Logout").WithName("Authentication-Logout");

            group.MediatePost<RequestEnforceLogoutByEmailCommand.Command>("/Logout/Request/Email").WithName("Authentication-Enforce-Logout-Email");

            group.MediatePost<RequestEnforceLogoutByPhoneNumberCommand.Command>("/Logout/Request/PhoneNumber").WithName("Authentication-Enforce-Logout-PhoneNumber");

            group.MediatePost<ConfirmEnforceLogoutCommand.Command>("Logout/Confirm").WithName("Authentication-Enforce-Logout-Confirm");
        }
    }
}
