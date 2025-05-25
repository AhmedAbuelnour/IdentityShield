using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Auth.Logout.Commands
{
    public static class LogoutCommand
    {
        public class Request
        {
            public required string RefreshToken { get; set; }
        }
        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                IdentityResult identityResult = await _userManager.LogoutAsync(request.Request.RefreshToken);

                if (identityResult.Succeeded)
                {
                    return TypedResults.Ok(OperationResult.Success);
                }
                else
                {
                    throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0028", "Refresh token not found!");
                }
            }
        }
    }
}
