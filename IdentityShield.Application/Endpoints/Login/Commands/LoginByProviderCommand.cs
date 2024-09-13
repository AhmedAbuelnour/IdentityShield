using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.Login.Commands
{

    public static class LoginByProviderCommand
    {
        public class Request
        {
            public required string Name { get; set; }
            public required string Value { get; set; }
        }

        public class Response
        {
            public string UserId { get; set; }
            public string? Email { get; set; }
            public bool? IsEmailVerified { get; set; }
            public string? PhoneNumber { get; set; }
            public bool? IsPhoneNumberVerified { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager,
                             JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByProviderAsync(request.Request.Name, request.Request.Value);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);

                return TypedResults.Ok(OperationResult<Response>.Succeed(new Response
                {
                    AccessToken = _jwtTokenService.GenerateJwtToken(user, userRoles.ToArray()),
                    RefreshToken = await _userManager.GenerateRefreshTokenAsync(user),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.Id,
                    IsEmailVerified = user.EmailConfirmed,
                    IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                }));
            }
        }
    }
}
