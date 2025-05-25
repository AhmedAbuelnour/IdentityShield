using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;

namespace IdentityShield.Application.Endpoints.Auth.Login.Commands
{
    public static class LoginByManagedAccountCommand
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string ManagedId { get; set; }
        }

        public class Response
        {
            public string UserId { get; set; }
            public string? Email { get; set; }
            public bool? IsEmailVerified { get; set; }
            public bool? IsPhoneNumberVerified { get; set; }
            public string? PhoneNumber { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager,
                             JwtTokenProvider<ShieldUser> _jwtTokenProvider) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser manager = await _userManager.FindByIdAsync(request.Request.ManagerId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                ShieldUser managed = await _userManager.FindByIdAsync(request.Request.ManagedId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                if (!await _userManager.IsAlreadyLinkedAsync(manager, managed))
                {
                    throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0032", "Managed account is not linked to the manager!");
                }

                IEnumerable<string> userRoles = await _userManager.GetRolesAsync(managed);

                JwtResponse jwtResponse = await _jwtTokenProvider.GetJwtAsync(managed, userRoles.ToArray(), cancellationToken);

                return TypedResults.Ok(OperationResult<Response>.Succeed(new Response
                {
                    AccessToken = jwtResponse.AccessToken,
                    RefreshToken = jwtResponse.RefreshToken,
                    Email = managed.Email,
                    PhoneNumber = managed.PhoneNumber,
                    UserId = managed.Id,
                    IsEmailVerified = managed.EmailConfirmed,
                    IsPhoneNumberVerified = managed.PhoneNumberConfirmed,
                }));
            }
        }
    }
}
