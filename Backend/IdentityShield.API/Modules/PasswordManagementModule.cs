using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.ChangePassword.Commands;
using IdentityShield.Application.Endpoints.AccessControl.ForgetPassword.Commands;
using IdentityShield.Application.Endpoints.AccessControl.ResetPassword.Commands;

namespace IdentityShield.API.Modules
{
    public class PasswordManagement : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Password").WithTags("Password-Management");

            group.MediatePost<ForgotPasswordByEmailCommand.Command>("/Forgot/Email").WithName("Password-Management-Forgot-Password-Email");
            group.MediatePost<ForgotPasswordByPhoneNumberCommand.Command>("/Forgot/PhoneNumber").WithName("Password-Management-Forgot-Password-PhoneNumber");

            group.MediatePost<ResetPasswordByEmailCommand.Command>("/Reset/Email").WithName("Password-Management-Reset-Password-Email");
            group.MediatePost<ResetPasswordByPhoneNumberCommand.Command>("/Reset/PhoneNumber").WithName("Password-Management-Reset-Password-PhoneNumber");

            group.MediatePost<ChangePasswordCommand.Command>("/Change").WithName("Password-Management-Change-Password");
        }
    }
}
