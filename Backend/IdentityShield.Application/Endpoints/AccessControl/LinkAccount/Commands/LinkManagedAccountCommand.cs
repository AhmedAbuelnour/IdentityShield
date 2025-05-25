using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Commands
{
    public static class LinkManagedAccountCommand
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

                if (await _userManager.IsAlreadyLinkedAsync(manager, managed))
                {
                    throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0031", "Managed account is already linked to the manager!");
                }

                await _userManager.LinkManagedAccountAsync(manager, managed);

                return TypedResults.Ok();

            }
        }
    }
}
