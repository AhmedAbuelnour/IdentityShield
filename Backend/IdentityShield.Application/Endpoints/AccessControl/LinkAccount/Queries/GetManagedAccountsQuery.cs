using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Queries
{
    public static class GetManagedAccountsQuery
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
            [FromRoute] public required string ManagerId { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Query>
        {
            public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
            {
                ShieldUser manager = await _userManager.FindByIdAsync(request.ManagerId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                IEnumerable<ShieldUser> managedAccounts = await _userManager.GetManagedAccountsAsync(manager);

                return TypedResults.Ok(managedAccounts.Select(a => new Response
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
