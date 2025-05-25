using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Commands
{
    public static class RemoveManagedAccountCommand
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string ManagedId { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public sealed class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser manager = await _userManager.FindByIdAsync(request.Request.ManagerId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                ShieldUser managed = await _userManager.FindByIdAsync(request.Request.ManagedId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                await _userManager.UnlinkManagedAccountAsync(manager, managed);

                return TypedResults.Ok();

            }
        }
    }
}
