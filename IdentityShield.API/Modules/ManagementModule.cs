using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.Management.ChangeEmail.Commands;
using IdentityShield.Application.Endpoints.Management.ChangePassword.Commands;
using IdentityShield.Application.Endpoints.Management.ChangePhoneNumber.Commands;
using IdentityShield.Application.Endpoints.Management.ConsumeRefreshToken.Commands;
using IdentityShield.Application.Endpoints.Management.ExternalProviders.Commands;
using IdentityShield.Application.Endpoints.Management.ForgetPassword.Commands;
using IdentityShield.Application.Endpoints.Management.ResendVerification.Commands;
using IdentityShield.Application.Endpoints.Management.ResetPassword.Commands;

namespace IdentityShield.API.Modules
{
    public class ManagementModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/Management").WithTags("Management");

            group.MediatePost<ForgotPasswordByEmailCommand.Command>("/ForgotPassword/Email").WithName("Management-Forgot-Password-Email");
            group.MediatePost<ForgotPasswordByPhoneNumberCommand.Command>("/ForgotPassword/PhoneNumber").WithName("Management-Forgot-Password-PhoneNumber");

            group.MediatePost<ResetPasswordByEmailCommand.Command>("/ResetPassword/Email").WithName("Management-Reset-Password-Email");
            group.MediatePost<ResetPasswordByPhoneNumberCommand.Command>("/ResetPassword/PhoneNumber").WithName("Management-Reset-Password-PhoneNumber");

            group.MediatePost<ResendVerificationByEmailCommand.Command>("/ResendVerification/Email").WithName("Management-Resend-Verification-Email");
            group.MediatePost<ResendVerificationByPhoneNumberCommand.Command>("/ResendVerification/PhoneNumber").WithName("Management-Resend-Verification-PhoneNumber");

            group.MediatePost<ChangePasswordCommand.Command>("/Change/Password").WithName("Management-Change-Password");


            group.MediatePost<LinkProviderCommand.Command>("/LinkProvider").WithName("Management-LinkProvider");
            group.MediatePost<UnlinkProviderCommand.Command>("/UnlinkProvider").WithName("Management-UnlinkProvider");

            group.MediatePost<RequestChangeEmailCommand.Command>("/RequestChange/Email").WithName("Management-Request-Change-Email");
            group.MediatePost<ConfirmChangeEmailCommand.Command>("/ConfirmChange/Email").WithName("Management-Confirm-Change-Email");

            group.MediatePost<RequestChangePhoneNumberCommand.Command>("/RequestChange/PhoneNumber").WithName("Management-Request-Change-PhoneNumber");
            group.MediatePost<ConfirmChangePhoneNumberCommand.Command>("/ConfirmChange/PhoneNumber").WithName("Management-Confirm-Change-PhoneNumber");


            group.MediatePost<ConsumeRefreshTokenCommand.Command>("/RefreshToken").WithName("Management-Consume-RefreshToken");
        }
    }
}
