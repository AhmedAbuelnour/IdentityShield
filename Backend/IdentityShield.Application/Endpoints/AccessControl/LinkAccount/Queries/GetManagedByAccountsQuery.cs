using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Queries
{
    public static class GetManagedByAccountsQuery
    {
        public class Response
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string? Email { get; internal set; }
            public string? PhoneNumber { get; internal set; }
        }

        public class Query : IEndPointRequest
        {
            [FromRoute] public required string ManagedId { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Query>
        {
            public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
            {
                ShieldUser managed = await _userManager.FindByIdAsync(request.ManagedId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                IEnumerable<ShieldUser> managedByAccounts = await _userManager.GetManagedByAccountsAsync(managed);

                return TypedResults.Ok(managedByAccounts.Select(a => new Response
                {
                    UserId = a.Id,
                    UserName = a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                }));
            }
        }
    }
}
