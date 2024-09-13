using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.User.Queries
{
    public static class GetUserInfoQuery
    {
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

        public class Query : IEndPointRequest
        {
            [FromRoute(Name = "UserId")] public required string UserId { get; set; }
        }

        public class Handler(ShieldUserManager _userManager,
                             JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Query>
        {
            public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }
                else
                {
                    IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);

                    return TypedResults.Ok(new OperationResult<Response>
                    {
                        Result = new Response
                        {
                            AccessToken = _jwtTokenService.GenerateJwtToken(user, userRoles.ToArray()),
                            RefreshToken = await _userManager.GenerateRefreshTokenAsync(user),
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
}
