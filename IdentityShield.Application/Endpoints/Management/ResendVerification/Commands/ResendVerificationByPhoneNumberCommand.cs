using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Management.ResendVerification.Commands
{
    public static class ResendVerificationByPhoneNumberCommand
    {
        public class Request
        {
            public required string Purpose { get; set; }
            public required string PhoneNumber { get; set; }
        }
        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager, INotificationService _notificationService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }


                if (await _userManager.GetTokenCountAsync(user, request.Request.Purpose, Constant.TokenTrackerProvider.PhoneNumber, TimeSpan.FromHours(24), cancellationToken) is int tokenCount && tokenCount > 5)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0012",
                        Description = "You have exceeded the maximum number of SMS/email messages per day, please try again tomorrow."
                    }));
                }

                // remove old ones,
                IdentityResult identityResult = await _userManager.RemoveAuthenticationTokenAsync(user, Constant.TokenProviders.OtpToken, request.Request.Purpose);

                string token = await _userManager.GenerateUserTokenAsync(user, Constant.TokenProviders.OtpToken, request.Request.Purpose);


                if (await _userManager.AddTokenAsync(user, request.Request.Purpose, token, Constant.TokenTrackerProvider.PhoneNumber, cancellationToken) > 0)
                {
                    // send otp
                    await _notificationService.SendSMSAsync(user, token, cancellationToken);
                }

                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
