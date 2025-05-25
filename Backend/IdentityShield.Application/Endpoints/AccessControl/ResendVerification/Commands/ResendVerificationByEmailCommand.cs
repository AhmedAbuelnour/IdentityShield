using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.ResendVerification.Commands
{
    public static class ResendVerificationByEmailCommand
    {
        public class Request
        {
            public required string Purpose { get; set; }
            public required string Email { get; set; }
        }
        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser user = await _userManager.FindByEmailAsync(request.Request.Email) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                // remove old ones,
                _ = await _userManager.RemoveAuthenticationTokenAsync(user, Constant.TokenProviders.OtpTokenProvider, request.Request.Purpose);

                await _userManager.SendEmailNotificationAsync(user, request.Request.Purpose, "Resent Verification Code - إعادة إرسال كود التفعيل", TimeSpan.FromHours(24));


                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }

}
