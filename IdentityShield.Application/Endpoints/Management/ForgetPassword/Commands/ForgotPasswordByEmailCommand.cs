using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.Management.ForgetPassword.Commands
{
    public static class ForgotPasswordByEmailCommand
    {
        public class Request
        {
            public required string Email { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager, INotificationService _notificationService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(request.Request.Email);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                if (await _userManager.GetTokenCountAsync(user, Constant.Purposes.ForgotPassword, Constant.TokenTrackerProvider.Email, TimeSpan.FromHours(24), cancellationToken) is int tokenCount && tokenCount > 5)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0012",
                        Description = "You have exceeded the maximum number of SMS/email messages per day, please try again tomorrow."
                    }));
                }


                string token = await _userManager.GenerateUserTokenAsync(user, Constant.TokenProviders.OtpToken, Constant.Purposes.ForgotPassword);

                // Send Email

                if (await _userManager.AddTokenAsync(user, Constant.Purposes.ForgotPassword, token, Constant.TokenTrackerProvider.Email, cancellationToken) > 0)
                {
                    // send otp
                    await _notificationService.SendEmailAsync(user, "Forgot Password - فقدان كلمة المرور", token, cancellationToken);
                }




                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
