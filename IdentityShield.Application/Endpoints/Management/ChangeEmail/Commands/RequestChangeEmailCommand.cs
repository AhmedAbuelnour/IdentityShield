using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.Management.ChangeEmail.Commands
{
    public static class RequestChangeEmailCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
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
                ApplicationUser? user = await _userManager.FindByIdAsync(request.Request.UserId);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                string token = await _userManager.GenerateUserTokenAsync(user, Constant.TokenProviders.OtpToken, Constant.Purposes.ChangeEmail);

                // Send Email

                if (await _userManager.AddTokenAsync(user, Constant.Purposes.ChangeEmail, token, Constant.TokenProviders.OtpToken, cancellationToken) > 0)
                {
                    // send otp
                    await _notificationService.SendEmailAsync(user.UserName, request.Request.Email, "سلاح التلميذ - تغير البريد الإلكترونى", token, cancellationToken);
                }




                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
