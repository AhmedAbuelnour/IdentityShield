using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Management.ChangeEmail.Commands
{
    public static class ConfirmChangeEmailCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
            public required string OTP { get; set; }
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
                ApplicationUser? user = await _userManager.FindByIdAsync(request.Request.UserId);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                if (!await _userManager.VerifyUserTokenAsync(user, Constant.TokenProviders.OtpToken, Constant.Purposes.ChangeEmail, request.Request.OTP))
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0009",
                        Description = "The verification code is expired or invalid, please try again."
                    }));
                }

                if (await _userManager.ChangeEmailAsync(user, request.Request.Email) is IdentityResult identityResult && !identityResult.Succeeded)
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
