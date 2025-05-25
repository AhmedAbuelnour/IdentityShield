using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;

namespace IdentityShield.Application.Endpoints.AccessControl.ConsumeRefreshToken.Commands
{
    public static class ConsumeRefreshTokenCommand
    {
        public class Request
        {
            public required string Token { get; set; }
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

        public class Handler(ShieldUserManager _shieldUserManager, JwtTokenProvider<ShieldUser> _jwtTokenProvider) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser? user = await _shieldUserManager.ConsumeRefreshTokenAsync(request.Request.Token);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                IEnumerable<string> userRoles = await _shieldUserManager.GetRolesAsync(user);

                JwtResponse jwtResponse = await _jwtTokenProvider.GetJwtAsync(user, userRoles.ToArray(), cancellationToken);

                return TypedResults.Ok(new OperationResult<Response>
                {
                    Result = new Response
                    {
                        AccessToken = jwtResponse.AccessToken,
                        RefreshToken = jwtResponse.RefreshToken,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserId = user.Id,
                        IsEmailVerified = user.EmailConfirmed,
                        IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                    }
                });
            }
        }
    }
}
