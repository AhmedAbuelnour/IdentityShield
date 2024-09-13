using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Register.Commands
{
    public static class ConfirmRegisterEmailCommand
    {
        public class Request
        {
            public required string Email { get; set; }
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
                ApplicationUser? user = await _userManager.FindByEmailAsync(request.Request.Email);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                if (!await _userManager.VerifyUserTokenAsync(user, Constant.TokenProviders.OtpToken, Constant.Purposes.Register, request.Request.OTP))
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0009",
                        Description = "The verification code is expired or invalid, please try again."
                    }));
                }

                if (await _userManager.ConfirmEmailAsync(user) is IdentityResult identityResult && !identityResult.Succeeded)
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
