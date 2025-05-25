using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.AccessControl.ChangePassword.Commands
{
    public static class ChangePasswordCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
            public required string OldPassword { get; set; }
            public required string NewPassword { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser user = await _userManager.FindByIdAsync(request.Request.UserId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!"); ;

                if (await _userManager.ChangePasswordAsync(user, request.Request.OldPassword, request.Request.NewPassword) is IdentityResult changePasswordResult && !changePasswordResult.Succeeded)
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
