using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.ChangeEmail.Commands;

namespace IdentityShield.API.Modules
{
    public class EmailManagementModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Email").WithTags("Email-Management");

            group.MediatePost<RequestChangeEmailCommand.Command>("/Change/Request").WithName("Email-Management-Change-Request");

            group.MediatePost<ConfirmChangeEmailCommand.Command>("/Change/Confirm").WithName("Email-Management-Change-Confirm");

        }
    }
}
