using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.AccessControl.ChangePhoneNumber.Commands
{

    public static class ConfirmChangePhoneNumberCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
            public required string OTP { get; set; }
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
                ShieldUser? user = await _userManager.FindByIdAsync(request.Request.UserId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!"); ;


                if (!await _userManager.VerifyUserTokenAsync(user, Constant.TokenProviders.OtpTokenProvider, Constant.Purposes.ChangePhoneNumber, request.Request.OTP))
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0009",
                        Description = "The verification code is expired or invalid, please try again."
                    }));
                }

                if (await _userManager.ChangePhoneNumberAsync(user, request.Request.PhoneNumber) is IdentityResult identityResult && !identityResult.Succeeded)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0001",
                        Description = "Invalid data. Please try again"
                    }));
                }



                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }

}
