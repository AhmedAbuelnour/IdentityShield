using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Management.ExternalProviders.Commands
{
    public static class LinkProviderCommand
    {
        public class Request
        {
            public required string UserId { get; set; }
            public required string Name { get; set; }
            public required string Value { get; set; }
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

                if (await _userManager.LinkProviderAsync(user, request.Request.Name, request.Request.Value) is IdentityResult identityResult && identityResult.Succeeded)
                {
                    return TypedResults.Ok(OperationResult.Success);
                }

                return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                {
                    Code = "XIDN0001",
                    Description = "Can't find the user with the provided data"
                }));
            }
        }
    }
}
