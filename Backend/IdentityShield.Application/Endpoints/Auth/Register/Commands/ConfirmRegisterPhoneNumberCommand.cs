using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Auth.Register.Commands
{
    public static class ConfirmRegisterPhoneNumberCommand
    {
        public class Request
        {
            public required string PhoneNumber { get; set; }
            public required string OTP { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!"); ;

                if (!await _userManager.VerifyUserTokenAsync(user, Constant.TokenProviders.OtpTokenProvider, Constant.Purposes.Register, request.Request.OTP))
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0009",
                        Description = "The verification code is expired or invalid, please try again."
                    }));
                }

                if (await _userManager.ConfirmPhoneNumberAsync(user) is IdentityResult identityResult && !identityResult.Succeeded)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0001",
                        Description = "Can't find the user with the provided data"
                    }));
                }

                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
