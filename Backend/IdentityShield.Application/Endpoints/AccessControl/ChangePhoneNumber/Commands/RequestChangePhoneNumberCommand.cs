using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.ChangePhoneNumber.Commands
{
    public static class RequestChangePhoneNumberCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
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
                ShieldUser user = await _userManager.FindByIdAsync(request.Request.UserId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                await _userManager.SendSMSNotificationAsync(user, Constant.Purposes.ChangePhoneNumber, TimeSpan.FromHours(24));

                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
