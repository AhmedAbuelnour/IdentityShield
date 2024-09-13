using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.Management.ConsumeRefreshToken.Commands
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
            [FromBody] public Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _shieldUserManager, JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _shieldUserManager.ConsumeRefreshTokenAsync(request.Request.Token);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }

                IEnumerable<string> roles = await _shieldUserManager.GetRolesAsync(user);

                return TypedResults.Ok(new OperationResult<Response>
                {
                    Result = new Response
                    {
                        AccessToken = _jwtTokenService.GenerateJwtToken(user, roles.ToArray()),
                        RefreshToken = await _shieldUserManager.GenerateRefreshTokenAsync(user),
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
