using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.ForgetPassword.Commands
{
    public static class ForgotPasswordByPhoneNumberCommand
    {
        public class Request
        {
            public required string PhoneNumber { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }


        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                await _userManager.SendSMSNotificationAsync(user, Constant.Purposes.ForgotPassword, TimeSpan.FromHours(24));

                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
