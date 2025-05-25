using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.ChangePhoneNumber.Commands;

namespace IdentityShield.API.Modules
{
    public class PhoneNumberManagementModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/PhoneNumber").WithTags("PhoneNumber-Management");

            group.MediatePost<RequestChangePhoneNumberCommand.Command>("/Change/Request").WithName("PhoneNumber-Management-Change-Request");

            group.MediatePost<ConfirmChangePhoneNumberCommand.Command>("/Change/Confirm").WithName("PhoneNumber-Management-Change-Confirm");
        }
    }
}
